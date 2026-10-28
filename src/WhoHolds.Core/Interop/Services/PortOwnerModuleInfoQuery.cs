using System.Diagnostics;
using System.Runtime.InteropServices;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

namespace WhoHolds.Core.Interop.Services;

internal static class PortOwnerModuleInfoQuery
{
    public static Result<(string? module, string? modulePath)> Query<TEntry>(TEntry entry)
        where TEntry : unmanaged, IPortRowOwnerModule
    {
        const TcpipOwnerModuleInfoClass basic = TcpipOwnerModuleInfoClass.Basic;

        QueryDelegate queryDelegate = entry switch
        {
            MibTcpRowOwnerModule e => (buffer, ref size) =>
                IpHlpApi.GetOwnerModuleFromTcpEntry(e, basic, buffer.Pointer, ref size),
            MibTcp6RowOwnerModule e => (buffer, ref size) =>
                IpHlpApi.GetOwnerModuleFromTcp6Entry(e, basic, buffer.Pointer, ref size),
            MibUdpRowOwnerModule e => (buffer, ref size) =>
                IpHlpApi.GetOwnerModuleFromUdpEntry(e, basic, buffer.Pointer, ref size),
            MibUdp6RowOwnerModule e => (buffer, ref size) =>
                IpHlpApi.GetOwnerModuleFromUdp6Entry(e, basic, buffer.Pointer, ref size),
            _ => throw new UnreachableException(),
        };

        var result = QueryWithRetry(queryDelegate, entry.OwnerProcessId);

        if (!result.Succeeded)
            return new ResultFailure(result.Errors);

        using var buffer = result.Value;
        var info = MemoryMarshal.Read<TcpipOwnerModuleBasicInfo>(result.Value);

        return Result.Success((info.ModuleName, info.ModulePath));
    }

    private static Result<NativeBuffer> QueryWithRetry(QueryDelegate queryDelegate, uint processId)
    {
        var initialBuffer = new NativeBuffer(1);
        uint size = (uint)initialBuffer.Size;

        var errorCode = queryDelegate.Invoke(initialBuffer, ref size);

        if (errorCode is SystemErrorCode.ERROR_SUCCESS)
            return Result.Success(initialBuffer);

        initialBuffer.Dispose();

        if (errorCode is SystemErrorCode.ERROR_INSUFFICIENT_BUFFER)
        {
            var buffer = new NativeBuffer((int)size);

            errorCode = queryDelegate.Invoke(buffer, ref size);

            if (errorCode is SystemErrorCode.ERROR_SUCCESS)
                return Result.Success(buffer);

            buffer.Dispose();
        }

        return Result.Failure(
            $"Failed to query module information ({processId} PID): {errorCode}."
        );
    }

    private delegate SystemErrorCode QueryDelegate(NativeBuffer buffer, ref uint size);
}

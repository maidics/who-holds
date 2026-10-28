using System.Net.Sockets;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Enums;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Interfaces;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Interop.Services;

internal static class PortOwnerModuleTableQuery
{
    public static Result<MibOwnerModuleTable<TRow>> Query<TRow>()
        where TRow : unmanaged, INativeSized<TRow>, IPortRowOwnerModule
    {
        var addressFamily = TRow.AddressFamily;

        var isTcp = TRow.ProtocolType == ProtocolType.Tcp;

        QueryDelegate queryDelegate = isTcp
            ? (buffer, ref size) =>
                IpHlpApi.GetExtendedTcpTable(
                    buffer,
                    ref size,
                    false,
                    addressFamily,
                    TcpTableClass.OwnerModuleAll,
                    0
                )
            : (buffer, ref size) =>
                IpHlpApi.GetExtendedUdpTable(
                    buffer,
                    ref size,
                    false,
                    addressFamily,
                    UdpTableClass.OwnerModule,
                    0
                );

        var queryResult = QueryWithGrowingBuffer(
            queryDelegate,
            isTcp ? nameof(IpHlpApi.GetExtendedTcpTable) : nameof(IpHlpApi.GetExtendedUdpTable)
        );

        if (!queryResult.Succeeded)
            return new ResultFailure(queryResult.Errors);

        using var buffer = queryResult.Value;

        return Result.Success(MibOwnerModuleTable<TRow>.FromBuffer(queryResult.Value));
    }

    private static Result<NativeBuffer> QueryWithGrowingBuffer(
        QueryDelegate queryDelegate,
        string method
    )
    {
        SystemErrorCode? errorCode = null;
        uint size = 64 * 1024; // at least 64 KB

        for (int i = 0; i < 5; i++)
        {
            var buffer = new NativeBuffer((int)size);

            errorCode = queryDelegate.Invoke(buffer, ref size);

            if (errorCode is SystemErrorCode.ERROR_SUCCESS)
                return Result.Success(buffer);

            buffer.Dispose();

            if (errorCode is SystemErrorCode.ERROR_INSUFFICIENT_BUFFER)
            {
                size = (uint)Math.Ceiling(size * 1.25); // 25% headroom for the returned size
                continue;
            }

            return Result.Failure($"{method} query failed: {errorCode}.");
        }

        return Result.Failure(
            $"Buffer size never converged for {method} after 5 attempts (last buffer: {ByteFormat.Humanize(size)}, status: {errorCode})"
        );
    }

    private delegate SystemErrorCode QueryDelegate(NativeBuffer buffer, ref uint size);
}

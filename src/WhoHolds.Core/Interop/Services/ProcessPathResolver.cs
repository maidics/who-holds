using System.Runtime.InteropServices;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.Kernel32Library;
using WhoHolds.Core.Interop.Kernel32Library.Enums;
using WhoHolds.Core.Interop.RestartManagerLibrary.Structs;

namespace WhoHolds.Core.Interop.Services;

internal static class ProcessPathResolver
{
    private const int _maxPathChars = 32_767;

    // this is not used for ports since GetOwnerModuleFromTcpEntry & GetOwnerModuleFromUdpEntry returns path
    public static Result<string> Resolve(RmProcessInfo holder)
    {
        using var process = Kernel32.OpenProcess(
            ProcessAccessRights.QueryInformation | ProcessAccessRights.DupHandle,
            false,
            holder.Process.dwProcessId
        );

        if (process.IsInvalid)
        {
            return Result.Failure(
                GetKernel32FunctionFailureMessage(
                    holder.AppName,
                    holder.Process.dwProcessId,
                    (SystemErrorCode)Marshal.GetLastPInvokeError(),
                    nameof(Kernel32.OpenProcess)
                )
            );
        }

        if (!Kernel32.GetProcessTimes(process, out var creationTime, out _, out _, out _))
            return Result.Failure(
                GetKernel32FunctionFailureMessage(
                    holder.AppName,
                    holder.Process.dwProcessId,
                    (SystemErrorCode)Marshal.GetLastPInvokeError(),
                    nameof(Kernel32.GetProcessTimes)
                )
            );

        if (creationTime.ToDateTimeUtc() != holder.Process.ProcessStartTime.ToDateTimeUtc())
            return Result.Failure(
                $"Process Id ({holder.Process.dwProcessId}) was reused, cannot resolve file path for process."
            );

        Span<char> path = stackalloc char[_maxPathChars]; // the upper limit is known (32KB)
        uint size = (uint)path.Length;

        if (!Kernel32.QueryFullProcessImageNameW(process, 0, path, ref size))
            return Result.Failure(
                GetKernel32FunctionFailureMessage(
                    holder.AppName,
                    holder.Process.dwProcessId,
                    (SystemErrorCode)Marshal.GetLastPInvokeError(),
                    nameof(Kernel32.QueryFullProcessImageNameW)
                )
            );

        return Result.Success<string>(path[..(int)size].ToString());
    }

    private static string GetKernel32FunctionFailureMessage(
        string? appName,
        uint processId,
        SystemErrorCode error,
        string function
    ) =>
        $"Failed to resolve image path for {appName} (PID {processId}): {error} ({function} - {Libraries.Kernel32})";
}

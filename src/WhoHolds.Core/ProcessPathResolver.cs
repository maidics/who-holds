using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Structs;

namespace WhoHolds.Core;

internal static class ProcessPathResolver
{
    public static Result<string> Resolve(RmProcessInfo holder)
    {
        using var process = NativeMethods.OpenProcess(
            ProcessAccessRights.QueryInformation | ProcessAccessRights.DupHandle,
            false,
            holder.Process.dwProcessId
        );

        if (process.IsInvalid)
        {
            var errorCode = (SystemErrorCode)Marshal.GetLastPInvokeError();

            return Result.Failure(
                $"{nameof(NativeMethods.OpenProcess)} failed to open process:  {errorCode}."
            );
        }

        if (!NativeMethods.GetProcessTimes(process, out var creationTime, out _, out _, out _))
            return Result.Failure(
                $"Failed to get creation time of process: {holder.Process.dwProcessId}."
            );

        if (creationTime.ToDateTimeUtc() != holder.Process.ProcessStartTime.ToDateTimeUtc())
            return Result.Failure("Process Id was reused, cannot resolve file path for process.");

        return SystemQuery.QueryImagePath(process, (int)holder.Process.dwProcessId);
    }
}

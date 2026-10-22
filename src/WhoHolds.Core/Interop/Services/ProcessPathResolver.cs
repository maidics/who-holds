using System.Runtime.InteropServices;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.Kernel32Library;
using WhoHolds.Core.Interop.Kernel32Library.Enums;
using WhoHolds.Core.Interop.RestartManagerLibrary.Structs;

namespace WhoHolds.Core.Interop.Services;

internal static class ProcessPathResolver // TODO: test this
{
    public static Result<string> Resolve(RmProcessInfo holder)
    {
        using var process = Kernel32.OpenProcess(
            ProcessAccessRights.QueryInformation | ProcessAccessRights.DupHandle,
            false,
            holder.Process.dwProcessId
        );

        if (process.IsInvalid)
        {
            var errorCode = (SystemErrorCode)Marshal.GetLastPInvokeError();

            return Result.Failure(
                $"{nameof(Kernel32.OpenProcess)} failed to open process:  {errorCode}."
            );
        }

        if (!Kernel32.GetProcessTimes(process, out var creationTime, out _, out _, out _))
            return Result.Failure(
                $"Failed to get creation time of process: {holder.Process.dwProcessId}."
            );

        if (creationTime.ToDateTimeUtc() != holder.Process.ProcessStartTime.ToDateTimeUtc())
            return Result.Failure("Process Id was reused, cannot resolve file path for process.");

        return SystemQuery.QueryImagePath(process, (int)holder.Process.dwProcessId);
    }
}

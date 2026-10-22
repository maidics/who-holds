using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop;

internal static partial class Kernel32
{
    /// <summary>
    /// Retrieves timing information for the specified process.
    /// </summary>
    /// <param name="hProcess">
    /// Handle opened with <see cref="ProcessAccessRights.QueryLimitedInformation"/>
    /// (or <see cref="ProcessAccessRights.QueryInformation"/>, which implies it).
    /// </param>
    /// <param name="lpCreationTime">Receives the creation time of the process (UTC).</param>
    /// <param name="lpExitTime">Receives the exit time. Undefined while the process is still running.</param>
    /// <param name="lpKernelTime">Receives the time spent executing in kernel mode.</param>
    /// <param name="lpUserTime">Receives the time spent executing in user mode.</param>
    /// <returns>
    /// <see langword="true"/> on success; otherwise read <see cref="Marshal.GetLastPInvokeError"/>.
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-getprocesstimes">
    /// GetProcessTimes function (processthreadsapi.h)
    /// </seealso>
    [LibraryImport(Libraries.Kernel32, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetProcessTimes(
        SafeProcessHandle hProcess,
        out FileTime lpCreationTime,
        out FileTime lpExitTime,
        out FileTime lpKernelTime,
        out FileTime lpUserTime
    );
}

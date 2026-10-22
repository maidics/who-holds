using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using WhoHolds.Core.Interop.Kernel32Library.Enums;

namespace WhoHolds.Core.Interop.Kernel32Library;

internal static partial class Kernel32
{
    /// <summary>
    /// Opens an existing local process object.
    /// </summary>
    /// <param name="dwDesiredAccess">
    /// Access rights requested on the process object, checked against its security descriptor.
    /// Request only what the caller uses; a single denied right fails the whole call.
    /// </param>
    /// <param name="bInheritHandle">
    /// Whether child processes created by the caller inherit the handle. Always <see langword="false"/> here.
    /// </param>
    /// <param name="dwProcessId">
    /// Identifier of the process to open. Native type is <c>DWORD</c>; .NET exposes process ids as <see cref="int"/>.
    /// </param>
    /// <returns>
    /// An open handle on success. On failure the handle is invalid (<c>NULL</c>, not <c>INVALID_HANDLE_VALUE</c>)
    /// and the error is available through <see cref="Marshal.GetLastPInvokeError"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// <c>ERROR_INVALID_PARAMETER</c>: the process does not exist or is the System Idle Process (0).
    /// <c>ERROR_ACCESS_DENIED</c>: the process exists but the rights were refused (elevated, other user,
    /// protected, System, CSRSS).
    /// </para>
    /// <para>
    /// A held handle keeps the process object alive, so its id cannot be reused until the handle is closed.
    /// </para>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-openprocess">
    /// OpenProcess function (processthreadsapi.h)
    /// </seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/procthread/process-security-and-access-rights">
    /// Process security and access rights
    /// </seealso>
    [LibraryImport(Libraries.Kernel32, SetLastError = true)] // kernel32.dll reports errors through the last-error value
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SafeProcessHandle OpenProcess(
        ProcessAccessRights dwDesiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
        uint dwProcessId
    );
}

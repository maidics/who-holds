using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop;

internal static partial class RestartManager
{
    /// <summary>
    /// Ends a Restart Manager session, releasing the session slot and its registry state.
    /// </summary>
    /// <param name="dwSessionHandle">
    /// Handle of an existing session, as returned by
    /// <see cref="RmStartSession"/> or <c>RmJoinSession</c>. Native type is <c>DWORD</c>:
    /// a plain 32-bit identifier, <em>not</em> a kernel <c>HANDLE</c>, so it must not be
    /// wrapped in a <see cref="SafeHandle"/> or closed with <c>CloseHandle</c>.
    /// </param>
    /// <returns>
    /// <see cref="SystemErrorCode"/>. Documented values are <c>ERROR_SUCCESS</c>,
    /// <c>ERROR_SEM_TIMEOUT</c>, <c>ERROR_WRITE_FAULT</c> (documented here as a registry
    /// read/write failure), <c>ERROR_OUTOFMEMORY</c> and <c>ERROR_INVALID_HANDLE</c>
    /// (no session exists for the supplied handle); handle unlisted codes defensively.
    /// </returns>
    /// <remarks>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/desktop/api/restartmanager/nf-restartmanager-rmendsession">
    /// RmEndSession function (restartmanager.h)
    /// </seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/desktop/Debug/system-error-codes">
    /// System error codes
    /// </seealso>
    [LibraryImport(Libraries.RestartManager)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode RmEndSession(uint dwSessionHandle);
}

using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop.RestartManagerLibrary;

internal static partial class RestartManager
{
    /// <summary>
    /// Starts a new Restart Manager session and returns a session handle and session key
    /// for use in subsequent Restart Manager calls.
    /// </summary>
    /// <param name="pSessionHandle">
    /// Receives the handle of the new session. Native type is <c>DWORD*</c>: a plain 32-bit
    /// identifier, <em>not</em> a kernel <c>HANDLE</c>, so it must not be wrapped in a
    /// <see cref="SafeHandle"/> or closed with <c>CloseHandle</c>. 0 is a valid handle. Release it with
    /// <see href="https://learn.microsoft.com/en-us/windows/desktop/api/restartmanager/nf-restartmanager-rmendsession">RmEndSession</see>.
    /// </param>
    /// <param name="dwSessionFlags">
    /// Reserved. Documented as required to be <c>0</c>; passing anything else is undefined.
    /// </param>
    /// <param name="strSessionKey">
    /// Caller-allocated buffer receiving the null-terminated session key. Must have room for
    /// <see cref="CCH_RM_SESSION_KEY"/> + 1 characters (33): the key is a GUID rendered as 32
    /// hex digits, plus the terminator. 66 bytes, so <c>stackalloc</c> is appropriate:
    /// <code>
    /// Span&lt;char&gt; key = stackalloc char[RestartManagerLimits.SessionKeyBufferLength];
    /// </code>
    /// </param>
    /// <returns>
    /// <see cref="SystemErrorCode"/>. Documented values are <c>ERROR_SUCCESS</c>,
    /// <c>ERROR_SEM_TIMEOUT</c>, <c>ERROR_BAD_ARGUMENTS</c>, <c>ERROR_MAX_SESSIONS_REACHED</c>,
    /// <c>ERROR_WRITE_FAULT</c> and <c>ERROR_OUTOFMEMORY</c>; handle unlisted codes defensively.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A maximum of 64 sessions per user session may be open simultaneously. Session state
    /// lives in the registry, not in the calling process, so every successful call must be
    /// paired with <c>RmEndSession</c> on all paths. Resetting this without ending the sessions
    /// requires reboot.
    /// </para>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmstartsession">
    /// RmStartSession function (restartmanager.h)
    /// </seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/desktop/Debug/system-error-codes">
    /// System error codes
    /// </seealso>
    [LibraryImport( // Native AOT and trimming works with LibraryImport, no StringBuilder, can step into marshalling code
        Libraries.RestartManager,
        StringMarshalling = StringMarshalling.Utf16 /* type of strSessionKey (char) is ambiguous: blittable to char and char16_t - this resolves it */
    )]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode RmStartSession(
        out uint pSessionHandle,
        uint dwSessionFlags,
        Span<char> strSessionKey // session key is used by other installers - this will not be used in this application
    );
}

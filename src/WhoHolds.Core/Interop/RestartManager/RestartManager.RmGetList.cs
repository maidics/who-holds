using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop;

internal static partial class RestartManager
{
    /// <summary>
    /// Gets a list of all applications and services that are currently using resources that have
    /// been registered with the Restart Manager session.
    /// </summary>
    /// <param name="dwSessionHandle">
    /// A handle to an existing Restart Manager session, obtained from <c>RmStartSession</c>.
    /// </param>
    /// <param name="pnProcInfoNeeded">
    /// Receives the array size required to hold all affected applications and services.
    /// </param>
    /// <param name="pnProcInfo">
    /// On input, the number of elements in <paramref name="rgAffectedApps"/>. On output, the number
    /// of structures actually written to the array.
    /// </param>
    /// <param name="rgAffectedApps">
    /// A caller-allocated array that receives the affected applications and services. Can be
    /// <see langword="null"/> if <paramref name="pnProcInfo"/> is 0.
    /// </param>
    /// <param name="lpdwRebootReasons">
    /// Receives a bitmask of <see cref="RmRebootReason"/> values explaining why a system restart
    /// would be needed, or <see cref="RmRebootReason.None"/> if none is needed.
    /// </param>
    /// <returns>
    /// <see cref="SystemErrorCode"/>
    /// </returns>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmgetlist"/>
    [LibraryImport(Libraries.RestartManager, StringMarshalling = StringMarshalling.Utf16)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode RmGetList(
        uint dwSessionHandle,
        out uint pnProcInfoNeeded,
        ref uint pnProcInfo,
        [In, Out] RmProcessInfo[]? rgAffectedApps,
        out RmRebootReason lpdwRebootReasons
    );
}

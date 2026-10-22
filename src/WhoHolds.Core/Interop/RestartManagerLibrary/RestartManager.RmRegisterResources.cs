using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.RestartManagerLibrary.Structs;

namespace WhoHolds.Core.Interop.RestartManagerLibrary;

internal static partial class RestartManager
{
    /// <summary>
    /// Registers resources to a Restart Manager session. The Restart Manager uses the list of
    /// resources registered with the session to determine which applications and services must be
    /// shut down and restarted. Resources can be identified by filenames, service short names, or
    /// <see cref="Structs.RmUniqueProcess"/> structures that describe running applications.
    /// </summary>
    /// <param name="dwSessionHandle">
    /// A handle to an existing Restart Manager session, obtained from <c>RmStartSession</c>.
    /// </param>
    /// <param name="nFiles">The number of files being registered.</param>
    /// <param name="rgsFileNames">
    /// An array of full file path names. This parameter can be <see langword="null"/> if
    /// <paramref name="nFiles"/> is 0.
    /// </param>
    /// <param name="nApplications">The number of processes being registered.</param>
    /// <param name="rgApplications">
    /// An array of <see cref="Structs.RmUniqueProcess"/> structures that identify the processes to register.
    /// This parameter can be <see langword="null"/> if <paramref name="nApplications"/> is 0.
    /// </param>
    /// <param name="nServices">The number of services being registered.</param>
    /// <param name="rgsServiceNames">
    /// An array of service short names. This parameter can be <see langword="null"/> if
    /// <paramref name="nServices"/> is 0.
    /// </param>
    /// <returns>
    /// <see cref="SystemErrorCode"/>
    /// </returns>
    /// <remarks>
    /// <para>
    /// Each call adds to the set of resources already registered with the session; resources
    /// cannot be unregistered. Register all resources of interest before calling <c>RmGetList</c>
    /// to determine which processes are using them.
    /// </para>
    /// <para>
    /// Strings are marshalled as UTF-16, matching the native <c>LPCWSTR</c> parameters.
    /// </para>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmregisterresources"/>
    [LibraryImport(Libraries.RestartManager, StringMarshalling = StringMarshalling.Utf16)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode RmRegisterResources(
        uint dwSessionHandle,
        uint nFiles,
        [MarshalUsing(CountElementName = nameof(nFiles))] [In] string[]? rgsFileNames,
        uint nApplications,
        [MarshalUsing(CountElementName = nameof(nApplications))]
        [In]
            RmUniqueProcess[]? rgApplications,
        uint nServices,
        [MarshalUsing(CountElementName = nameof(nServices))] [In] string[]? rgsServiceNames
    );
}

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Structs;

namespace WhoHolds.Core.Interop;

/// <summary>
/// Raw P/Invoke declarations for the Windows API: <c>ntdll.dll</c>, <c>rstrtmgr.dll</c>.
/// </summary>
/// <remarks>
/// <para>
/// Signatures only. Members mirror the native declarations one-to-one, including naming and capitalization, per the .NET interop guidance.
/// </para>
/// </remarks>
/// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices">
/// Native interoperability best practices
/// </seealso>
internal static partial class NativeMethods
{
    private const string NtDll = "ntdll.dll";
    private const string RestartManager = "rstrtmgr.dll";

    /// <summary>
    /// Retrieves the specified system information from the kernel.
    /// </summary>
    /// <param name="systemInformationClass">
    /// Selects which structure the kernel returns. This is the only type information in the
    /// call — the kernel does not validate that <paramref name="buffer"/> matches
    /// the class, it simply writes the bytes for whatever class is named here.
    /// </param>
    /// <param name="buffer">
    /// Caller-allocated buffer that receives the data. Must remain valid for at least
    /// <paramref name="bufferLength"/> bytes; the kernel takes that length on trust,
    /// so understating the allocation corrupts the heap rather than returning an error.
    /// May be <see cref="IntPtr.Zero"/> when the length is 0, which is useful for probing
    /// whether a class exists on the current build.
    /// </param>
    /// <param name="bufferLength">
    /// Capacity of the buffer in <b>bytes</b>. If the data does not fit, nothing
    /// is written and <see cref="NtStatus.InfoLengthMismatch"/> is returned. Native type is
    /// <c>ULONG</c>; never pass a negative value, as it reinterprets as a huge unsigned size.
    /// </param>
    /// <param name="returnLength">
    /// On success, the number of bytes written. On <see cref="NtStatus.InfoLengthMismatch"/>,
    /// the number of bytes required — but for variable-length classes such as
    /// <see cref="InformationClass.ExtendedHandle"/> this is frequently 0, and is stale
    /// even when populated because the underlying tables change between calls. Treat it as a
    /// hint for the growth factor, not a size to allocate exactly.
    /// </param>
    /// <returns>
    /// <see cref="NtStatus.ERROR_SUCCESS"/>, or an NTSTATUS error code. Failure is indicated by the
    /// high bit being set; values in the <c>0x40000000</c> range are informational successes.
    /// Common results are <see cref="NtStatus.InfoLengthMismatch"/> (0xC0000004, buffer too
    /// small) and <see cref="NtStatus.InvalidInfoClass"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This routine is undocumented apart from a handful of classes and may change between
    /// Windows releases. Validate struct layouts against the target build rather than assuming
    /// them.
    /// </para>
    /// <para>
    /// <b>Does not set the Win32 last-error value.</b> ntdll routines report failure solely
    /// through NTSTATUS, so do not declare <c>SetLastError</c> and do not call
    /// <see cref="Marshal.GetLastWin32Error"/> afterwards — it would return a stale value from
    /// an unrelated call. Use <c>RtlNtStatusToDosError</c> if a Win32 code is needed for display.
    /// </para>
    /// <para>
    /// Variable-length classes require a grow-and-retry loop, since the data can also change
    /// size between the failed call and the retry:
    /// <code>
    /// int size = 1024 * 1024;
    /// while (true)
    /// {
    ///     IntPtr buffer = Marshal.AllocHGlobal(size);
    ///     var status = NtQuerySystemInfo(systemInformationClass, buffer, size, out int needed);
    ///     if (status != NtStatus.InfoLengthMismatch)
    ///         return (status, buffer, needed);   // caller frees
    ///     Marshal.FreeHGlobal(buffer);
    ///     size = Math.Max(needed, size * 2);
    /// }
    /// </code>
    /// Cap the iterations so a persistent failure cannot spiral into gigabyte allocations.
    /// </para>
    /// <para>
    /// The caller owns <paramref name="buffer"/> in all cases and must free it,
    /// including on every failed attempt.
    /// </para>
    /// </remarks>
    [DllImport(NtDll, EntryPoint = "NtQuerySystemInformation")]
    internal static extern NtStatus NtQuerySystemInfo(
        int systemInformationClass,
        IntPtr buffer,
        int bufferLength,
        out uint returnLength
    );

    [DllImport(NtDll, EntryPoint = "NtQueryObject")]
    internal static extern NtStatus NtQueryObject(
        IntPtr handle,
        int objectInformationClass,
        IntPtr buffer,
        int bufferLength,
        out uint returnLength
    );

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
        RestartManager,
        StringMarshalling = StringMarshalling.Utf16 /* type of strSessionKey (char) is ambiguous: blittable to char and char16_t - this resolves it */
    )]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode RmStartSession(
        out uint pSessionHandle,
        uint dwSessionFlags,
        Span<char> strSessionKey // session key is used by other installers - this will not be used in this application
    );

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
    [LibraryImport(RestartManager)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode RmEndSession(uint dwSessionHandle);

    /// <summary>
    /// Registers resources to a Restart Manager session. The Restart Manager uses the list of
    /// resources registered with the session to determine which applications and services must be
    /// shut down and restarted. Resources can be identified by filenames, service short names, or
    /// <see cref="RmUniqueProcess"/> structures that describe running applications.
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
    /// An array of <see cref="RmUniqueProcess"/> structures that identify the processes to register.
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
    [LibraryImport(RestartManager, StringMarshalling = StringMarshalling.Utf16)]
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
    [LibraryImport(RestartManager, StringMarshalling = StringMarshalling.Utf16)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial SystemErrorCode RmGetList(
        uint dwSessionHandle,
        out uint pnProcInfoNeeded,
        ref uint pnProcInfo,
        [In, Out] RmProcessInfo[]? rgAffectedApps,
        out RmRebootReason lpdwRebootReasons
    );
}

using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Interop;

internal static class NativeMethods
{
    private const string NtDll = "ntdll.dll";

    /// <summary>
    /// Retrieves the specified system information from the kernel.
    /// </summary>
    /// <param name="systemInformationClass">
    /// Selects which structure the kernel returns. This is the only type information in the
    /// call — the kernel does not validate that <paramref name="systemInformation"/> matches
    /// the class, it simply writes the bytes for whatever class is named here.
    /// </param>
    /// <param name="systemInformation">
    /// Caller-allocated buffer that receives the data. Must remain valid for at least
    /// <paramref name="systemInformationLength"/> bytes; the kernel takes that length on trust,
    /// so understating the allocation corrupts the heap rather than returning an error.
    /// May be <see cref="IntPtr.Zero"/> when the length is 0, which is useful for probing
    /// whether a class exists on the current build.
    /// </param>
    /// <param name="systemInformationLength">
    /// Capacity of the buffer in <b>bytes</b>. If the data does not fit, nothing
    /// is written and <see cref="NtStatus.InfoLengthMismatch"/> is returned. Native type is
    /// <c>ULONG</c>; never pass a negative value, as it reinterprets as a huge unsigned size.
    /// </param>
    /// <param name="returnLength">
    /// On success, the number of bytes written. On <see cref="NtStatus.InfoLengthMismatch"/>,
    /// the number of bytes required — but for variable-length classes such as
    /// <see cref="SystemInformationClass.ExtendedHandle"/> this is frequently 0, and is stale
    /// even when populated because the underlying tables change between calls. Treat it as a
    /// hint for the growth factor, not a size to allocate exactly.
    /// </param>
    /// <returns>
    /// <see cref="NtStatus.Success"/>, or an NTSTATUS error code. Failure is indicated by the
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
    ///     var status = NtQuerySystemInfo(cls, buffer, size, out int needed);
    ///     if (status != NtStatus.InfoLengthMismatch)
    ///         return (status, buffer, needed);   // caller frees
    ///     Marshal.FreeHGlobal(buffer);
    ///     size = Math.Max(needed, size * 2);
    /// }
    /// </code>
    /// Cap the iterations so a persistent failure cannot spiral into gigabyte allocations.
    /// </para>
    /// <para>
    /// The caller owns <paramref name="systemInformation"/> in all cases and must free it,
    /// including on every failed attempt.
    /// </para>
    /// </remarks>
    [DllImport(NtDll, EntryPoint = "NtQuerySystemInformation")]
    internal static extern NtStatus NtQuerySystemInfo(
        SystemInformationClass systemInformationClass,
        IntPtr systemInformation,
        int systemInformationLength,
        out int returnLength
    );
}

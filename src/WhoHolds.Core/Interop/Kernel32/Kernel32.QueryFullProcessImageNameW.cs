using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace WhoHolds.Core.Interop;

internal static partial class Kernel32
{
    [LibraryImport(Libraries.Kernel32, SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool QueryFullProcessImageNameW(
        SafeProcessHandle hProcess,
        uint dwFlags, // 0 = Win32 path (C:\...)
        Span<char> lpExeName,
        ref uint lpdwSize // in: buffer capacity in chars; out: chars written, excluding the terminator
    );
}

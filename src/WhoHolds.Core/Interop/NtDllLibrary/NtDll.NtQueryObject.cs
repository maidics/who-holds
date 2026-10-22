using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.NtDllLibrary.Enums;

namespace WhoHolds.Core.Interop.NtDllLibrary;

internal static partial class NtDll
{
    [DllImport(Libraries.NtDll, EntryPoint = "NtQueryObject")]
    internal static extern NtStatus NtQueryObject(
        IntPtr handle,
        int objectInformationClass,
        IntPtr buffer,
        int bufferLength,
        out uint returnLength
    );
}

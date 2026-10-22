using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop;

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

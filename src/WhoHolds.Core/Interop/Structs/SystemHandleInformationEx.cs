using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct SystemHandleInformationEx
{
    public IntPtr NumberOfHandles;
    public IntPtr Reserved;
}

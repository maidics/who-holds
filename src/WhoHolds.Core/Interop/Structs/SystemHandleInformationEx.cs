using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct SystemHandleInformationEx : INtStruct
{
    public IntPtr NumberOfHandles;
    public IntPtr Reserved;

    public override string ToString()
    {
        return $"{nameof(SystemHandleInformationEx)}:\n\t- NumberOfHandles: {NumberOfHandles.ToInt32()}\n\t- Reserved: {Reserved.ToInt32()}";
    }

    public static int GetSize() => Marshal.SizeOf<SystemHandleInformationEx>();
}

using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct SystemHandleInformationEx : INativeSized<SystemHandleInformationEx>
{
    public IntPtr NumberOfHandles;
    public IntPtr Reserved;

    public override string ToString()
    {
        return $"{nameof(SystemHandleInformationEx)}:\n\t- NumberOfHandles: {NumberOfHandles.ToInt32()}\n\t- Reserved: {Reserved.ToInt32()}";
    }

    public static int Size => Marshal.SizeOf<SystemHandleInformationEx>();
}

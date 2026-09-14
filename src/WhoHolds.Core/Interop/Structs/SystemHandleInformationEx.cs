using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct SystemHandleInformationEx
{
    public IntPtr NumberOfHandles;
    public IntPtr Reserved;

    public override string ToString()
    {
        return $"{nameof(SystemHandleInformationEx)}:\n\t- NumberOfHandles: {NumberOfHandles.ToInt32()}\n\t- Reserved: {Reserved.ToInt32()}";
    }
}

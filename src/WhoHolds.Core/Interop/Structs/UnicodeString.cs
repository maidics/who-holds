using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct UnicodeString : INtStruct
{
    public ushort Length,
        MaximumLength; // 2 + 2 bytes but it pads to 8 bytes, bcos of Buffer below
    public IntPtr Buffer;

    public static int GetSize() => Marshal.SizeOf<UnicodeString>();
}

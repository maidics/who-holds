using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct UnicodeString : INativeSized<UnicodeString>
{
    public ushort Length,
        MaximumLength; // 2 + 2 bytes but it pads to 8 bytes, bcos of Buffer below
    public IntPtr Buffer;

    public static int Size => Marshal.SizeOf<UnicodeString>();
}

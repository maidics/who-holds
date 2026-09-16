using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct GenericMapping : INativeSized<GenericMapping>
{
    public uint R,
        W,
        E,
        A;

    public static int Size => Marshal.SizeOf<GenericMapping>();
}

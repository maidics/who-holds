using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop.NtDllLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
internal struct GenericMapping : INativeSized<GenericMapping>
{
    public uint R,
        W,
        E,
        A;

    public static int Size => Marshal.SizeOf<GenericMapping>();
}

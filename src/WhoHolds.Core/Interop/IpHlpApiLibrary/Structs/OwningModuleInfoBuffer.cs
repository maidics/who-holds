using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Constants;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
[InlineArray(IpHlpApiLimits.OwningModuleSize)]
internal struct OwningModuleInfoBuffer : INativeSized<OwningModuleInfoBuffer>
{
    private ulong _element0;

    public static int Size => Marshal.SizeOf<OwningModuleInfoBuffer>();
}

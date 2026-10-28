using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.IpHlpApiLibrary.Constants;

namespace WhoHolds.Core.Interop.IpHlpApiLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
[InlineArray(IpHlpApiLimits.OwningModuleSize)]
internal struct OwningModuleInfoBuffer
{
    private ulong _element0;
}

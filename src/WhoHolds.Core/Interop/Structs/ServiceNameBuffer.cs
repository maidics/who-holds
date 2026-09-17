using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[InlineArray(RestartManagerLimits.ServiceNameBufferLength)]
internal struct ServiceNameBuffer : INativeSized<ServiceNameBuffer>
{
    private ushort _element0; // using ushort which then gets casted to char

    public static int Size => Marshal.SizeOf<ServiceNameBuffer>();
}

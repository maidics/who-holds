using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop;

[InlineArray(RestartManagerLimits.ServiceNameBufferLength)]
internal struct ServiceNameBuffer : INativeSized<ServiceNameBuffer>
{
    private ushort _element0; // using ushort which then gets casted to char

    public static int Size => Marshal.SizeOf<ServiceNameBuffer>();
}

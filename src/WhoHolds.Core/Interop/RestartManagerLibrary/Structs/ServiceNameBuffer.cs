using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.RestartManagerLibrary.Constants;

namespace WhoHolds.Core.Interop.RestartManagerLibrary.Structs;

[InlineArray(RestartManagerLimits.ServiceNameBufferLength)]
internal struct ServiceNameBuffer : INativeSized<ServiceNameBuffer>
{
    private ushort _element0; // using ushort which then gets casted to char

    public static int Size => Marshal.SizeOf<ServiceNameBuffer>();
}

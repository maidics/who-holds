using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Constants;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
[InlineArray(RestartManagerLimits.MaxAppNameBufferLength)]
internal struct AppNameBuffer : INativeSized<AppNameBuffer>
{
    private ushort _element0; // using ushort which then gets cast to char

    public static int Size => Marshal.SizeOf<AppNameBuffer>();
}

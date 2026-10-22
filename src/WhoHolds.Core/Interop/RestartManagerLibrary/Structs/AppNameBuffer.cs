using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;
using WhoHolds.Core.Interop.RestartManagerLibrary.Constants;

namespace WhoHolds.Core.Interop.RestartManagerLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
[InlineArray(RestartManagerLimits.MaxAppNameBufferLength)]
internal struct AppNameBuffer : INativeSized<AppNameBuffer>
{
    private ushort _element0; // using ushort which then gets cast to char

    public static int Size => Marshal.SizeOf<AppNameBuffer>();
}

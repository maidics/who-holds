using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.RestartManagerLibrary.Constants;

namespace WhoHolds.Core.Interop.RestartManagerLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
[InlineArray(RestartManagerLimits.MaxAppNameBufferLength)]
internal struct AppNameBuffer
{
    private ushort _element0; // using ushort which then gets cast to char
}

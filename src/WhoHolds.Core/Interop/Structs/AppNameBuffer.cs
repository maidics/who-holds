using System.Runtime.CompilerServices;
using WhoHolds.Core.Interop.Constants;

namespace WhoHolds.Core.Interop.Structs;

[InlineArray(RestartManagerLimits.MaxAppNameBufferLength)]
internal struct AppNameBuffer
{
    private ushort _element0; // using ushort which then gets casted to char
}

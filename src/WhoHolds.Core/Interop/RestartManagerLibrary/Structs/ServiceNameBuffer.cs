using System.Runtime.CompilerServices;
using WhoHolds.Core.Interop.RestartManagerLibrary.Constants;

namespace WhoHolds.Core.Interop.RestartManagerLibrary.Structs;

[InlineArray(RestartManagerLimits.ServiceNameBufferLength)]
internal struct ServiceNameBuffer
{
    private ushort _element0; // using ushort which then gets casted to char
}

using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Common;

namespace WhoHolds.Core.Interop.RestartManagerLibrary.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct RmUniqueProcess
{
    public readonly uint dwProcessId;
    public readonly FileTime ProcessStartTime;
}

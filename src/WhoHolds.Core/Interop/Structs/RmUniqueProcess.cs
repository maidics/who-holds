using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct RmUniqueProcess
{
    public readonly uint dwProcessId;
    public readonly FileTime ProcessStartTime;
}

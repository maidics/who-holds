using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct RmUniqueProcess : INativeSized<RmUniqueProcess>
{
    public readonly uint dwProcessId;
    public readonly FileTime ProcessStartTime;

    public static int Size => Marshal.SizeOf<RmUniqueProcess>();
}

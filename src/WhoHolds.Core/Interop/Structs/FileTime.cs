using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct FileTime : INativeSized<FileTime>
{
    public readonly uint dwLowDateTime; // lower 32 bits
    public readonly uint dwHighDateTime; // upper 32 bits

    public DateTime ToDateTimeUtc()
    {
        var ticks = ((long)dwHighDateTime << 32) | dwLowDateTime;
        return DateTime.FromFileTimeUtc(ticks);
    }

    public static int Size => Marshal.SizeOf<FileTime>();

    // test constructor
    internal FileTime(uint lowDateTime, uint highDateTime)
    {
        dwLowDateTime = lowDateTime;
        dwHighDateTime = highDateTime;
    }
}

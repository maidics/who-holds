using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop.Common;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct FileTime
{
    public readonly uint dwLowDateTime; // lower 32 bits
    public readonly uint dwHighDateTime; // upper 32 bits

    public DateTime ToDateTimeUtc()
    {
        var ticks = ((long)dwHighDateTime << 32) | dwLowDateTime;
        return DateTime.FromFileTimeUtc(ticks);
    }

    // test constructor
    internal FileTime(uint lowDateTime, uint highDateTime)
    {
        dwLowDateTime = lowDateTime;
        dwHighDateTime = highDateTime;
    }
}

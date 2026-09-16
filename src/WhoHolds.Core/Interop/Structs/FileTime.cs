using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct FileTime : INativeSized<FileTime>
{
    public readonly uint dwLowDateTime;
    public readonly uint dwHighDateTime;

    public static int Size => Marshal.SizeOf<FileTime>();
}

using System.Runtime.InteropServices;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct FileTime
{
    public readonly uint dwLowDateTime;
    public readonly uint dwHighDateTime;
}

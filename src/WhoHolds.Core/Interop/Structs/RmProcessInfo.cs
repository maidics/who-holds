using System.Runtime.InteropServices;
using WhoHolds.Core.Interop.Enums;

namespace WhoHolds.Core.Interop.Structs;

// TODO: do testing on structs
[StructLayout(LayoutKind.Sequential)]
internal readonly struct RmProcessInfo
{
    public readonly RmUniqueProcess Process;
    public readonly AppNameBuffer strAppName;
    public readonly ServiceNameBuffer strServiceShortName;
    public readonly RmAppType ApplicationType;
    public readonly RmAppStatus AppStatus;
    public readonly uint TSSessionId;
    public readonly int Restartable;

    public string AppName => ReadString(MemoryMarshal.Cast<ushort, char>(strAppName));
    public string ServiceShortName =>
        ReadString(MemoryMarshal.Cast<ushort, char>(strServiceShortName));
    public bool IsRestartable => Restartable != 0;

    private static string ReadString(ReadOnlySpan<char> buffer)
    {
        int end = buffer.IndexOf('\0');
        return new string(end < 0 ? buffer : buffer[..end]);
    }
}

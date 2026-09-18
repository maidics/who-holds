using System.Runtime.InteropServices;
using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Enums;
using WhoHolds.Core.Interop.Interfaces;

namespace WhoHolds.Core.Interop.Structs;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct RmProcessInfo : INativeSized<RmProcessInfo>
{
    public readonly RmUniqueProcess Process;
    public readonly AppNameBuffer strAppName;
    public readonly ServiceNameBuffer strServiceShortName;
    public readonly RmAppType ApplicationType;
    public readonly RmAppStatus AppStatus;
    public readonly uint TSSessionId;
    public readonly int Restartable;

    public string? AppName => ReadString(MemoryMarshal.Cast<ushort, char>(strAppName));
    public string? ServiceShortName =>
        ReadString(MemoryMarshal.Cast<ushort, char>(strServiceShortName));
    public bool IsRestartable => Restartable != 0;
    public int? SessionId => TSSessionId == uint.MaxValue ? null : (int)TSSessionId;

    private static string? ReadString(ReadOnlySpan<char> buffer)
    {
        int end = buffer.IndexOf('\0');

        return end switch
        {
            0 => null, // starts with terminator
            < 0 => new string(buffer), // no terminator: use whole buffer
            _ => new string(
                buffer[
                    ..end /* range operator: from the start up to but not including end */
                ]
            ), // not only terminator: use buffer without terminator
        };
    }

    public static int Size => Marshal.SizeOf<RmProcessInfo>();

    public HolderProcess ToHolderProcess()
    {
        return new HolderProcess(
            (int)Process.dwProcessId,
            Process.ProcessStartTime.ToDateTimeUtc(),
            AppName,
            ServiceShortName,
            ApplicationType.ToString(),
            AppStatusToStrings(),
            SessionId,
            IsRestartable
        );
    }

    private List<string> AppStatusToStrings()
    {
        if (AppStatus is RmAppStatus.StatusUnknown)
            return ["Unknown"];

        var names = new List<string>();
        var remaining = (uint)AppStatus;

        foreach (var flag in Enum.GetValues<RmAppStatus>())
        {
            if (flag == RmAppStatus.StatusUnknown || !AppStatus.HasFlag(flag))
                continue;

            names.Add(
                flag switch
                {
                    RmAppStatus.StatusRunning => "Running",
                    RmAppStatus.StatusStopped => "Stopped",
                    RmAppStatus.StatusStoppedOther => "Stopped by another process",
                    RmAppStatus.StatusRestarted => "Restarted",
                    RmAppStatus.StatusErrorOnStop => "Error on stop",
                    RmAppStatus.StatusErrorOnRestart => "Error on restart",
                    RmAppStatus.StatusShutdownMasked => "Shutdown masked",
                    RmAppStatus.StatusRestartMasked => "Restart masked",
                    _ => flag.ToString(),
                }
            );

            remaining &= ~(uint)flag; // ~ bitwise not; & bitwise AND => clears flag's bit from remaining
        }

        if (remaining != 0)
            names.Add($"Unrecognized (0x{remaining:X})");

        return names;
    }

    // test constructor
    internal RmProcessInfo(
        RmUniqueProcess process,
        string appName,
        string? serviceShortName,
        RmAppType applicationType,
        RmAppStatus appStatus,
        uint sessionId,
        int restartable
    )
    {
        Process = process;

        var appNameBuffer = new AppNameBuffer();
        Span<ushort> appNameSpan = appNameBuffer;
        WriteString(appName, MemoryMarshal.Cast<ushort, char>(appNameSpan));
        strAppName = appNameBuffer;

        var serviceNameBuffer = new ServiceNameBuffer();
        Span<ushort> serviceNameSpan = serviceNameBuffer;
        WriteString(serviceShortName, MemoryMarshal.Cast<ushort, char>(serviceNameSpan));
        strServiceShortName = serviceNameBuffer;

        ApplicationType = applicationType;
        AppStatus = appStatus;
        TSSessionId = sessionId;
        Restartable = restartable;
    }

    private static void WriteString(string? value, Span<char> buffer)
    {
        if (string.IsNullOrEmpty(value))
            return; // buffer is already zeroed, so it's an empty string

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value.Length, buffer.Length);

        value.AsSpan().CopyTo(buffer);
        buffer[value.Length] = '\0';
    }
}

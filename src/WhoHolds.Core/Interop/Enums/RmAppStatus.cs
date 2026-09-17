namespace WhoHolds.Core.Interop.Enums;

internal enum RmAppStatus : uint
{
    StatusUnknown = 0x0,
    StatusRunning = 0x1,
    StatusStopped = 0x2,
    StatusStoppedOther = 0x4,
    StatusRestarted = 0x8,
    StatusErrorOnStop = 0x10,
    StatusErrorOnRestart = 0x20,
    StatusShutdownMasked = 0x40,
    StatusRestartMasked = 0x80,
}

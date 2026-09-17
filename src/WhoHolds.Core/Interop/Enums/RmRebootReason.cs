namespace WhoHolds.Core.Interop.Enums;

[Flags]
internal enum RmRebootReason : uint
{
    None = 0x0,
    PermissionDenied = 0x1,
    SessionMismatch = 0x2,
    CriticalProcess = 0x4,
    CriticalService = 0x8,
    DetectedSelf = 0x10,
}

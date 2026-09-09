namespace WhoHolds.Core.Interop;

internal enum NtStatus : uint
{
    Success = 0x00000000,
    InvalidInfoClass = 0xC0000003,
    InfoLengthMismatch = 0xC0000004,
    AccessDenied = 0xC0000022,
    InvalidHandle = 0xC0000008,
}

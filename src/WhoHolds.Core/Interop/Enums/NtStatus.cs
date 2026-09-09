namespace WhoHolds.Core.Interop.Enums;

/// <summary>
/// NTSTATUS codes returned by NT native API calls.
/// </summary>
/// <remarks>
/// NTSTATUS is natively a signed 32-bit value (<c>typedef LONG NTSTATUS</c>). This enum uses
/// <see cref="uint"/> as the underlying type so error codes can be written in their conventional
/// hexadecimal form; the bit patterns are identical either way.
/// The high bits encode severity: 0x0 success, 0x4 informational, 0x8 warning, 0xC error.
/// </remarks>
internal enum NtStatus : uint // 32 bits exactly but the compiler wouldn't know whether the first bit is read as sign or magnitude
// -> this is why uint is a good choice for the enum
// 0x - hex number, 0 - decimal number, 0b - binary number
{
    Success = 0x00000000,
    InvalidInfoClass = 0xC0000003, // the class does not exist on the build
    InfoLengthMismatch = 0xC0000004, // the buffer is not big enough for the data
    AccessDenied = 0xC0000022,
    InvalidHandle = 0xC0000008,
}

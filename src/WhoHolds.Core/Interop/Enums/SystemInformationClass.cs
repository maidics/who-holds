namespace WhoHolds.Core.Interop.Enums;

internal enum SystemInformationClass
{
    /// <summary>
    /// SYSTEM_BASIC_INFORMATION. Fixed size.
    /// </summary>
    Basic = 0,

    /// <summary>
    /// SYSTEM_HANDLE_INFORMATION. Legacy handle table. PID is a ushort.
    /// </summary>
    Handle = 16,

    /// <summary>
    /// SYSTEM_HANDLE_INFORMATION_EX. System-wide handle table.
    /// </summary>
    ExtendedHandle = 64,
}

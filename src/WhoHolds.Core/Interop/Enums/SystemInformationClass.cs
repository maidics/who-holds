namespace WhoHolds.Core.Interop.Enums;

internal enum SystemInformationClass
{
    /// <summary>
    /// SYSTEM_BASIC_INFORMATION. Fixed size.
    /// </summary>
    Basic = 0,

    /// <summary>
    /// OBJECT_TYPES_INFORMATION.
    /// </summary>
    ObjectTypesInformation = 3,

    /// <summary>
    /// SYSTEM_HANDLE_INFORMATION. Legacy handle table. PID is an ushort.
    /// </summary>
    Handle = 16,

    /// <summary>
    /// SYSTEM_HANDLE_INFORMATION_EX. System-wide handle table.
    /// </summary>
    ExtendedHandle = 64,
}

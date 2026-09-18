namespace WhoHolds.Core.Interop.Enums;

internal enum SystemErrorCode : uint
{
    /// <summary>
    /// The function completed successfully.
    /// </summary>
    ERROR_SUCCESS = 0,

    /// <summary>
    /// A path registered to the Restart Manager session is a directory.
    /// </summary>
    ERROR_ACCESS_DENIED = 5,

    /// <summary>
    /// No Restart Manager session exists with the given handle.
    /// </summary>
    ERROR_INVALID_HANDLE = 6,

    /// <summary>
    /// Not enough memory was available to complete the operation.
    /// </summary>
    ERROR_OUTOFMEMORY = 14,

    /// <summary>
    /// The system cannot write to the specified device.
    /// </summary>
    ERROR_WRITE_FAULT = 29,

    /// <summary>
    /// Restart Manager could not obtain the Registry write mutex in the allotted time.
    /// A system restart is recommended because further use of the Restart Manager is likely to fail.
    /// </summary>
    ERROR_SEM_TIMEOUT = 121,

    /// <summary>
    /// One or more arguments are not correct.
    /// </summary>
    ERROR_BAD_ARGUMENTS = 160,

    /// <summary>
    /// Passed buffer is too small to hold all the information.
    /// </summary>
    ERROR_MORE_DATA = 234,

    /// <summary>
    /// The maximum number of sessions has been reached: 64.
    /// </summary>
    ERROR_MAX_SESSIONS_REACHED = 353,

    /// <summary>
    /// Operation canceled by the user.
    /// </summary>
    ERROR_CANCELLED = 1223,
}

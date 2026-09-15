namespace WhoHolds.Core.Interop.Enums;

public enum SystemErrorCode : uint
{
    /// <summary>
    /// ERROR_SUCCESS - the function completed successfully.
    /// </summary>
    Success = 0,

    /// <summary>
    /// ERROR_OUTOFMEMORY - not enough memory was available to complete the operation.
    /// </summary>
    OutOfMemory = 14,

    /// <summary>
    /// ERROR_WRITE_FAULT - the system cannot write to the specified device.
    /// </summary>
    WriteFault = 29,

    /// <summary>
    /// ERROR_SEM_TIMEOUT - Restart Manager could not obtain the Registry write mutex in the allotted time.
    /// A system restart is recommended because further use of the Restart Manager is likely to fail.
    /// </summary>
    Timeout = 121,

    /// <summary>
    /// ERROR_BAD_ARGUMENTS - one or more arguments are not correct.
    /// </summary>
    BadArguments = 160,

    /// <summary>
    /// ERROR_MAX_SESSIONS_REACHED - the maximum number of sessions has been reached: 64.
    /// </summary>
    MaxSessionsReached = 353,
}

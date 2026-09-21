namespace WhoHolds.Cli.Output;

/// <summary>
/// Process exit codes.
/// </summary>
public static class ExitCodes
{
    /// <summary>No process is holding the specified file.</summary>
    public const int NotLocked = 0;

    /// <summary>At least one process is holding one of the specified files.</summary>
    public const int Locked = 1;

    /// <summary>The check could not be completed: invalid arguments,
    /// a Restart Manager failure, or another unexpected error.</summary>
    public const int Error = 2;
}

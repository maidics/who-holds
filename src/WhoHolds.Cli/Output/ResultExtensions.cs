using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public static class ResultExtensions
{
    extension(Result<HolderProcess[]> result)
    {
        public int GetExitCode()
        {
            return !result.Succeeded ? ExitCodes.Error
                : result.Value.Length > 0 ? ExitCodes.Locked
                : ExitCodes.NotLocked;
        }
    }
}

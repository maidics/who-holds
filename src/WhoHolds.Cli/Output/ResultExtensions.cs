using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public static class ResultExtensions
{
    extension<T>(Result<T[]> result)
    {
        public int GetExitCode()
        {
            return !result.Succeeded ? ExitCodes.Error
                : result.Value.Length > 0 ? ExitCodes.Held
                : ExitCodes.NotHeld;
        }
    }
}

using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core;

public static class WhoHolds
{
    public static Result<HolderProcess[]> File(string file)
    {
        var fileCheckResult = PathUtils.CheckFilePath(file);

        if (!fileCheckResult.Succeeded)
            return Result.Failure(fileCheckResult.Errors);

        using var session = new RestartManagerSession([file]);
        var startResult = session.Start();

        if (!startResult.Succeeded)
            return Result.Failure(startResult.Errors);

        var getResult = session.GetProcesses(out _);

        if (!getResult.Succeeded)
            return Result.Failure(getResult.Errors);

        var processes = getResult.Value.Select(p => p.ToHolderProcess()).ToArray();

        return Result.Success(processes);
    }
}

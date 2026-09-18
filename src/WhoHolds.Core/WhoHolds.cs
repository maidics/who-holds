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
            return fileCheckResult.ToFailure<HolderProcess[]>();

        using var session = new RestartManagerSession([file]);
        session.Start();

        var getResult = session.GetProcesses(out _);

        if (!getResult.Succeeded)
            return getResult.ToFailure<HolderProcess[]>();

        var processes = getResult.Value.Select(p => p.ToHolderProcess()).ToArray();

        return Result.Success(processes);
    }
}

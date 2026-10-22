using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Interop.Services;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core;

public static class WhoHolds
{
    public static Result<HolderProcess[]> File(string file)
    {
        var resolutionResult = PathUtils.ResolveFullPath(file);

        if (!resolutionResult.Succeeded)
            return Result.Failure(resolutionResult.Errors);

        using var session = new RestartManagerSession([resolutionResult.Value]);
        var startResult = session.Start();

        if (!startResult.Succeeded)
            return Result.Failure(startResult.Errors);

        var getResult = session.GetProcesses(out _);

        if (!getResult.Succeeded)
            return Result.Failure(getResult.Errors);

        // TODO: test after this
        List<string> pathErrors = [];

        var processes = getResult
            .Value.Select(p =>
            {
                var pathResult = ProcessPathResolver.Resolve(p);

                if (pathResult.Succeeded)
                {
                    return p.ToHolderProcess(pathResult.Value);
                }

                pathErrors.AddRange(pathResult.Errors);
                return p.ToHolderProcess(null);
            })
            .ToArray();

        return Result.Success(processes, pathErrors.ToArray());
    }
}

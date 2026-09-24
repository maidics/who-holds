using ModularPipelines.Context;
using ModularPipelines.Models;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Interfaces;

namespace WhoHolds.Pipeline.Requirements;

public sealed class CppBuildToolRequirement(ICppBuildToolLocator locator) : IPipelineRequirement
{
    public async Task<RequirementDecision> MustAsync(IPipelineHookContext context)
    {
        var result = await locator.LocateAsync(context);

        if (!result.VsWhereFound)
            return RequirementDecision.Failed(
                $"{Repo.VsWhere} not found. Install Visual Studio or Build Tools with the 'Desktop development with C++' workload."
            );

        if (result.InstallationPath is null)
            return RequirementDecision.Failed(
                "C++ build tools (MSVC x64) not found. Native AOT publish requires the 'Desktop development with C++' workload."
            );

        return RequirementDecision.Passed;
    }
}

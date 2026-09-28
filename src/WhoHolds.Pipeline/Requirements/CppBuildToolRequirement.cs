using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Models;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Requirements;

public sealed class CppBuildToolRequirement(
    ICppBuildToolLocator locator,
    IOptions<PipelineSettings> pipelineOptions,
    IOptions<CppBuildToolSettings> cppBuildToolOptions
) : IPipelineRequirement
{
    public async Task<RequirementDecision> MustAsync(IPipelineHookContext context)
    {
        if (!pipelineOptions.Value.IsTagPush)
        {
            context.Logger.LogSkippingRequirementOnNonTagPush<CppBuildToolRequirement>();
            return RequirementDecision.Passed;
        }

        var result = await locator.LocateAsync(context);

        if (!result.VsWhereFound)
            return RequirementDecision.Failed(
                $"{cppBuildToolOptions.Value.VsWhere} not found. Install Visual Studio or Build Tools with the 'Desktop development with C++' workload."
            );

        if (result.InstallationPath is null)
            return RequirementDecision.Failed(
                "C++ build tools (MSVC x64) not found. Native AOT publish requires the 'Desktop development with C++' workload."
            );

        return RequirementDecision.Passed;
    }
}

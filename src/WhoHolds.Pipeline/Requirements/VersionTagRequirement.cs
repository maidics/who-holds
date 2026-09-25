using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularPipelines.Context;
using ModularPipelines.Models;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Extensions;

namespace WhoHolds.Pipeline.Requirements;

public sealed partial class VersionTagFormatRequirement(IConfiguration configuration)
    : IPipelineRequirement // this will not be a requirement if not a tag push
{
    // vMAJOR.MINOR.PATCH, digits only, no leading zeros, nothing before or after - required for production
    [GeneratedRegex(@"^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\z")]
    private static partial Regex Pattern();

    public Task<RequirementDecision> MustAsync(IPipelineHookContext context)
    {
        if (!configuration.IsTagPush())
        {
            context.Logger.LogSkippingRequirementOnNonTagPush<VersionTagFormatRequirement>();
            return RequirementDecision.Passed;
        }

        var tag = configuration.GetRequiredStringValue(Repo.GitHubRefNameEnvVar);

        if (!IsValid(tag))
            return RequirementDecision.Failed(
                $"Invalid tag: '{tag}'. Tag must have the following format: 'v1.0.0'."
            );

        return RequirementDecision.Passed;
    }

    private static bool IsValid(string tag) => Pattern().IsMatch(tag);
}

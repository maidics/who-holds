using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.Models;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Requirements;

public sealed partial class VersionTagFormatRequirement(IOptions<PipelineSettings> options)
    : IPipelineRequirement // this will not be a requirement if not a tag push
{
    // vMAJOR.MINOR.PATCH, digits only, no leading zeros, nothing before or after - required for production
    [GeneratedRegex(@"^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\z")]
    private static partial Regex Pattern();

    public Task<RequirementDecision> MustAsync(IPipelineHookContext context)
    {
        if (!options.Value.IsTagPush)
        {
            context.Logger.LogSkippingRequirementOnNonTagPush<VersionTagFormatRequirement>();
            return RequirementDecision.Passed;
        }

        if (!IsValid(options.Value.GitHubRefName))
            return RequirementDecision.Failed(
                $"Invalid tag: '{options.Value.GitHubRefName}'. Tag must have the following format: 'v1.0.0'."
            );

        return RequirementDecision.Passed;
    }

    private static bool IsValid(string tag) => Pattern().IsMatch(tag);
}

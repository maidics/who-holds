using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Models;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Requirements;

public sealed partial class TagRequirement(IConfiguration configuration) : IPipelineRequirement // this will not be a requirement if not a tag push
{
    [GeneratedRegex(@"^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\z")] // vMAJOR.MINOR.PATCH, digits only, no leading zeros, nothing before or after - required for production
    private static partial Regex Pattern();

    public Task<RequirementDecision> MustAsync(IPipelineHookContext context)
    {
        var tag = configuration.GetValue<string>(Repo.GitHubRefNameEnvVar);

        if (string.IsNullOrEmpty(tag))
            return RequirementDecision.Failed(
                $"Configuration not found: '{Repo.GitHubRefNameEnvVar}'."
            );

        if (!IsValid(tag))
            return RequirementDecision.Failed(
                $"Invalid tag: '{tag}'. Tag must have the following format: 'v1.0.0'."
            );

        return RequirementDecision.Passed;
    }

    private static bool IsValid(string tag) => Pattern().IsMatch(tag);
}

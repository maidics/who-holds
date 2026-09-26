using Microsoft.Extensions.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Models;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Requirements;

public sealed class ConfigurationRequirement(IConfiguration configuration) : IPipelineRequirement
{
    public static readonly string[] Required =
    [
        Repo.GitHubRefTypeEnvVar,
        Repo.GitHubRefNameEnvVar,
        Repo.PublishOutputDirectory,
    ];

    public Task<RequirementDecision> MustAsync(IPipelineHookContext context)
    {
        var missing = Required
            .Where(k => string.IsNullOrWhiteSpace(configuration[k]))
            .Order()
            .ToList();

        return missing.Count == 0
            ? RequirementDecision.Passed
            : RequirementDecision.Failed(
                $"Missing required configurations: {string.Join(", ", missing)}."
            );
    }
}

using Microsoft.Extensions.Configuration;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Requirements;

namespace WhoHolds.Pipeline.Tests.Requirements;

public sealed class ConfigurationRequirementTests
{
    [Test]
    [Arguments(Repo.GitHubRefTypeEnvVar)]
    [Arguments(Repo.GitHubRefNameEnvVar)]
    public async Task ShouldReturnFailedIfMissingConfigurationKeys(params string[] keys)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(keys.ToDictionary(key => key, string? (_) => "test"))
            .Build();

        var missing = ConfigurationRequirement
            .Required.Where(r => !keys.Contains(r))
            .Order()
            .ToList();

        var requirement = new ConfigurationRequirement(config);

        var result = await requirement.MustAsync(null!);
        result.Success.ShouldBeFalse();
        result.Reason.ShouldNotBeNull();
        result.Reason.ShouldBe($"Missing required configurations: {string.Join(", ", missing)}.");
    }

    [Test]
    public async Task ShouldReturnFailedIfHasConfigurationKeysButAreWhiteSpace()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                ConfigurationRequirement.Required.ToDictionary(key => key, string? (_) => " ")
            )
            .Build();

        var missing = ConfigurationRequirement.Required.Order().ToList();

        var requirement = new ConfigurationRequirement(config);

        var result = await requirement.MustAsync(null!);
        result.Success.ShouldBeFalse();
        result.Reason.ShouldNotBeNull();
        result.ShouldBe($"Missing required configurations: {string.Join(", ", missing)}.");
    }

    [Test]
    public async Task ShouldReturnPassedWhenAllKeysArePresent()
    {
        const string v = "test";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    [Repo.GitHubRefTypeEnvVar] = v,
                    [Repo.GitHubRefNameEnvVar] = v,
                }
            )
            .Build();

        var requirement = new ConfigurationRequirement(config);

        var result = await requirement.MustAsync(null!);
        result.Success.ShouldBeTrue();
    }
}

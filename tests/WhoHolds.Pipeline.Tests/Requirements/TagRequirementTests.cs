using Microsoft.Extensions.Configuration;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Requirements;

namespace WhoHolds.Pipeline.Tests.Requirements;

public sealed class TagRequirementTests
{
    [Test]
    [Arguments("")]
    [Arguments(null)]
    public async Task ShouldReturnFailedIfRefNameIsMissingOrEmpty(string? refName)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

        var requirement = new TagRequirement(config);

        var result = await requirement.MustAsync(null!);
        result.Success.ShouldBeFalse();
        result.Reason.ShouldNotBeNull();
        result.Reason.ShouldContain("Configuration not found");
    }

    [Test]
    [Arguments("test")]
    [Arguments("1-1-1")]
    [Arguments("1_1_1")]
    [Arguments("1.1.1.1")]
    [Arguments("v1.1.1.1")]
    public async Task ShouldReturnFailedWhenTagIsNotValid(string tag)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { [Repo.GitHubRefNameEnvVar] = tag }
            )
            .Build();

        var requirement = new TagRequirement(config);

        var result = await requirement.MustAsync(null!);
        result.Success.ShouldBeFalse();
        result.Reason.ShouldNotBeNull();
        result.Reason.ShouldContain($"Invalid tag: '{tag}'");
    }

    [Test]
    [Arguments("v0.0.0")]
    [Arguments("v1.2.0")]
    public async Task ShouldPassIfTagIsValid(string tag)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { [Repo.GitHubRefNameEnvVar] = tag }
            )
            .Build();

        var requirement = new TagRequirement(config);

        var result = await requirement.MustAsync(null!);
        result.Success.ShouldBeTrue();
        result.Reason.ShouldBeNull();
    }
}

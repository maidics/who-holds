using Microsoft.Extensions.Configuration;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Extensions;

namespace WhoHolds.Pipeline.Tests.Extensions;

public sealed class ConfigurationExtensionsTests
{
    [Test]
    public void GetRequiredStringValueShouldThrowIfKeyIsMissing()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

        Should.Throw<KeyNotFoundException>(() => config.GetRequiredStringValue("key"));
    }

    [Test]
    [Arguments("")]
    [Arguments(" ")]
    [Arguments("\n")]
    public void GetRequiredStringValueShouldThrowIfValueIsEmptyOrWhiteSpace(string value)
    {
        const string key = nameof(key);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { [key] = value })
            .Build();

        Should.Throw<KeyNotFoundException>(() => config.GetRequiredStringValue(key));
    }

    [Test]
    public void GetRequiredStringValueShouldReturnValueIfKeyIsPresent()
    {
        const string key = nameof(key);
        const string value = nameof(value);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { [key] = value })
            .Build();

        var v = config.GetRequiredStringValue(key);
        v.ShouldBe(value);
    }

    [Test]
    public void IsTagPushShouldThrowIfGitHubRefTypeKeyIsMissing()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

        Should.Throw<KeyNotFoundException>(() => config.IsTagPush());
    }

    [Test]
    public void IsTagPushShouldReturnFalseIfRefTypeIsNotTag()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { [Repo.GitHubRefTypeEnvVar] = "not-tag" }
            )
            .Build();

        config.IsTagPush().ShouldBeFalse();
    }

    [Test]
    public void IsTagPushShouldReturnTrueIfRefTypeIsTag()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { [Repo.GitHubRefTypeEnvVar] = Repo.GitHubTagRef }
            )
            .Build();

        config.IsTagPush().ShouldBeTrue();
    }
}

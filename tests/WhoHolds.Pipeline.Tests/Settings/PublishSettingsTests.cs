using Microsoft.Extensions.Configuration;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Tests.Settings;

public sealed class PublishSettingsTests
{
    [Test]
    public void ShouldThrowIfOutputDirectoryDoesNotExist()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    [Repo.GitHubRefNameEnvVar] = "test",
                    [Repo.PublishOutputDirectory] = "test",
                }
            )
            .Build();

        Should.Throw<DirectoryNotFoundException>(() => PublishSettings.From(config));
    }

    [Test]
    public void ShouldCreateFromConfiguration()
    {
        const string refName = nameof(refName);
        string output = AppContext.BaseDirectory;

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    [Repo.GitHubRefNameEnvVar] = refName,
                    [Repo.PublishOutputDirectory] = output,
                }
            )
            .Build();

        var settings = PublishSettings.From(config);
        settings.TagName.ShouldBe(refName);
        settings.OutputDirectory.ShouldBe(output);
    }
}

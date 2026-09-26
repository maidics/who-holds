using Microsoft.Extensions.Configuration;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Extensions;

namespace WhoHolds.Pipeline.Settings;

public sealed record PublishSettings(string TagName, string OutputDirectory)
{
    public static PublishSettings From(IConfiguration configuration)
    {
        var tag = configuration.GetRequiredStringValue(Repo.GitHubRefNameEnvVar);

        var output = configuration.GetRequiredStringValue(Repo.PublishOutputDirectory); // TODO: add to config requirement

        if (!Directory.Exists(output))
            throw new DirectoryNotFoundException($"Invalid publish output directory: '{output}'.");

        return new PublishSettings(tag, output);
    }
}

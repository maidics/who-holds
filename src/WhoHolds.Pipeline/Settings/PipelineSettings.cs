using Microsoft.Extensions.Configuration;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Settings;

public sealed record PipelineSettings(string? ReleaseVersion)
{
    public bool IsTagPush => ReleaseVersion is not null;

    public static PipelineSettings From(IConfiguration configuration)
    {
        var isTagPush =
            configuration.GetValue<string>(Repo.GitHubRefTypeEnvVar) == Repo.GitHubTagRef;

        var tag = isTagPush
            ? configuration.GetValue<string>(Repo.GitHubTagRef)
                ?? throw new InvalidOperationException(
                    $"'{Repo.GitHubRefNameEnvVar}' environment variable not found."
                )
            : null;

        return new PipelineSettings(tag is not null ? ParseVersion(tag) : null);
    }

    private static string ParseVersion(string tag)
    {
        // Build == -1 means only 2 parts; Revision != -1 means 4 parts
        if (
            !tag.StartsWith('v')
            || !Version.TryParse(tag[1..], out var version)
            || version.Build == -1
            || version.Revision != -1
        )
        {
            throw new InvalidOperationException(
                $"Tag '{tag}' is not a valid version. Expected e.g. 'v1.2.0'."
            );
        }

        return version.ToString();
    }
}

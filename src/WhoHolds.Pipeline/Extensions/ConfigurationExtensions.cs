using Microsoft.Extensions.Configuration;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Extensions;

public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public bool IsTagPush()
        {
            var refType = configuration.GetValue<string>(Repo.GitHubRefTypeEnvVar);

            if (string.IsNullOrEmpty(refType))
                throw new InvalidOperationException(
                    $"Configuration not found: '{Repo.GitHubRefTypeEnvVar}'."
                );

            return refType == Repo.GitHubTagRef;
        }
    }
}

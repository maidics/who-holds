using Microsoft.Extensions.Configuration;
using WhoHolds.Pipeline.Constants;

namespace WhoHolds.Pipeline.Extensions;

public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public string GetRequiredStringValue(string key)
        {
            var value = configuration.GetValue<string>(key);

            if (string.IsNullOrWhiteSpace(value))
                throw new KeyNotFoundException(
                    $"Required configuration not found: '{key}', value: '{value}'."
                );

            return value;
        }

        public bool IsTagPush()
        {
            return configuration.GetRequiredStringValue(Repo.GitHubRefTypeEnvVar)
                == Repo.GitHubTagRef;
        }
    }
}

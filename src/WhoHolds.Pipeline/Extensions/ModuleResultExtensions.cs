using ModularPipelines.Models;
using WhoHolds.Pipeline.Models;

namespace WhoHolds.Pipeline.Extensions;

public static class ModuleResultExtensions
{
    extension(ModuleResult<PublishedBuild?> result)
    {
        public PublishedBuild EnsurePublished()
        {
            var publishedBuild = result.ValueOrDefault;
            ArgumentNullException.ThrowIfNull(publishedBuild);

            if (!File.Exists(publishedBuild.FilePath))
                throw new FileNotFoundException(
                    $"Published file not found at path: '{publishedBuild.FilePath}'."
                );

            return publishedBuild;
        }
    }
}

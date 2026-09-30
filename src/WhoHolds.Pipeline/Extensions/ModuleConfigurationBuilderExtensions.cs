using ModularPipelines.Configuration;
using ModularPipelines.Models;

namespace WhoHolds.Pipeline.Extensions;

public static class ModuleConfigurationBuilderExtensions
{
    extension(ModuleConfigurationBuilder builder) // TODO: call this in tag push modules
    {
        public ModuleConfigurationBuilder WithTagPushSkip(bool isTagPush) =>
            builder.WithSkipWhen(_ =>
                isTagPush ? SkipDecision.DoNotSkip : SkipDecision.Skip("Only runs on tag pushes.")
            );
    }
}

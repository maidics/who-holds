using Microsoft.Extensions.Logging;
using ModularPipelines.Logging;
using ModularPipelines.Requirements;

namespace WhoHolds.Pipeline.Extensions;

public static class ModuleLoggerExtensions
{
    extension(IModuleLogger logger)
    {
        public void LogSkippingRequirementOnNonTagPush<TRequirement>()
            where TRequirement : IPipelineRequirement =>
            logger.LogInformation(
                "{Requirement} returning early on non tag ref push.",
                typeof(TRequirement).Name
            );
    }
}

using Microsoft.Extensions.Logging;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Requirements;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Extensions;

public sealed class ModuleLoggerExtensionsTests
{
    [Test]
    public void LogSkippingRequirementOnNonTagPushShouldLog()
    {
        var logger = new FakeModuleLogger();
        logger.LogSkippingRequirementOnNonTagPush<CppBuildToolRequirement>();
        logger.Collector.Count.ShouldBe(1);
        logger.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.LatestRecord.Message.ShouldBe(
            $"{nameof(CppBuildToolRequirement)} returning early on non tag ref push."
        );
    }
}

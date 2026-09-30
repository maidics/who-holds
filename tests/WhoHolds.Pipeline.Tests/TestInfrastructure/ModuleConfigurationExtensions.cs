using ModularPipelines.Configuration;
using ModularPipelines.Context;

namespace WhoHolds.Pipeline.Tests.TestInfrastructure;

public static class ModuleConfigurationExtensions
{
    extension(ModuleConfiguration config)
    {
        public async Task ShouldSkipOnTagPushAsync(
            bool isTagPush,
            IModuleContext? moduleContext = null
        )
        {
            config.SkipCondition.ShouldNotBeNull();
            var result = await config.SkipCondition.Invoke(moduleContext ?? null!);
            result.Reason.ShouldBe(isTagPush ? null : "Only runs on tag pushes.");
            result.ShouldSkip.ShouldBe(!isTagPush);
        }
    }
}

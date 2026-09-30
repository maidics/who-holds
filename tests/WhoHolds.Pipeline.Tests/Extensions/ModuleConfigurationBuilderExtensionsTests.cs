using ModularPipelines.Configuration;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Extensions;

public sealed class ModuleConfigurationBuilderExtensionsTests
{
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task ShouldAddSkipOnTagPush(bool isTagPush)
    {
        var config = ModuleConfiguration.Create().WithTagPushSkip(isTagPush).Build();
        await config.ShouldSkipOnTagPushAsync(isTagPush);
    }
}

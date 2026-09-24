using ModularPipelines.Models;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Requirements;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Requirements;

public sealed class CppBuildToolRequirementTests
{
    private static Task<RequirementDecision> EvaluateAsync(CppBuildToolLookupResult result) =>
        new CppBuildToolRequirement(new TestCppBuildToolLocator(result)).MustAsync(null!);

    [Test]
    public async Task ShouldReturnFailedWhenVsWhereIsNotFound()
    {
        var decision = await EvaluateAsync(new CppBuildToolLookupResult(false, null));
        decision.Success.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldReturnFailedWhenVsWhereIsFoundButCppToolsAreMissing()
    {
        var decision = await EvaluateAsync(new CppBuildToolLookupResult(true, null));
        decision.Success.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldReturnSuccessWhenVsWhereAndCppToolsAreFound()
    {
        var decision = await EvaluateAsync(new CppBuildToolLookupResult(true, "test"));
        decision.Success.ShouldBeTrue();
    }
}

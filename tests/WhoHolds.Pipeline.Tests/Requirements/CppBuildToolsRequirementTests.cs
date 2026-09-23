using ModularPipelines.Models;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Requirements;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Requirements;

public sealed class CppBuildToolsRequirementTests
{
    private static Task<RequirementDecision> EvaluateAsync(CppBuildToolsLookupResult result) =>
        new CppBuildToolsRequirement(new TestCppBuildToolsLocator(result)).MustAsync(null!);

    [Test]
    public async Task ShouldReturnFailedWhenVsWhereIsNotFound()
    {
        var decision = await EvaluateAsync(new CppBuildToolsLookupResult(false, null));
        decision.Success.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldReturnFailedWhenVsWhereIsFoundButCppToolsAreMissing()
    {
        var decision = await EvaluateAsync(new CppBuildToolsLookupResult(true, null));
        decision.Success.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldReturnSuccessWhenVsWhereAndCppToolsAreFound()
    {
        var decision = await EvaluateAsync(new CppBuildToolsLookupResult(true, "test"));
        decision.Success.ShouldBeTrue();
    }
}

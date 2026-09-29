using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class DraftReleaseModuleTests
{
    [Test]
    public void ShouldDependOnSmokeTestModule()
    {
        DraftReleaseModule.ShouldHaveDependsOnAttribute<DraftReleaseModule, SmokeTestModule>();
    }
}

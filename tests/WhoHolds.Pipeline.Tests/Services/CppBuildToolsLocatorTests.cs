using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Services;

namespace WhoHolds.Pipeline.Tests.Services;

public sealed class CppBuildToolsLocatorTests
{
    [Test]
    public async Task ShouldReturnMissingVsWhereIfNotFound()
    {
        var locator = new CppBuildToolsLocator(
            Path.Combine(AppContext.BaseDirectory, Repo.VsWhere)
        );
        var result = await locator.LocateAsync(null!);
        result.VsWhereFound.ShouldBeFalse();
        result.InstallationPath.ShouldBeNull();
    }

    // Faking an IPipelineContext is not worth it, so the verification is done when the ci runs
}

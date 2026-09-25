using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Services;

namespace WhoHolds.Pipeline.Tests.Services;

public sealed class CppBuildToolLocatorTests
{
    [Test]
    public void DefaultPathShouldBeCorrect()
    {
        Path.GetFileName(CppBuildToolLocator.DefaultPath).ShouldBe(Repo.VsWhere);

        CppBuildToolLocator.DefaultPath.ShouldEndWith(
            Path.Combine("Microsoft Visual Studio", "Installer", Repo.VsWhere)
        );

        Path.Exists(CppBuildToolLocator.DefaultPath).ShouldBeTrue();
    }

    [Test]
    public async Task ShouldReturnNotFoundIfVsWhereNotFound()
    {
        var path = Path.Combine(AppContext.BaseDirectory, Repo.VsWhere);

        var locator = new CppBuildToolLocator(path);

        var result = await locator.LocateAsync(null!);
        result.VsWhereFound.ShouldBeFalse();
        result.InstallationPath.ShouldBeNull();
    }

    [Test]
    public async Task ShouldReturnPathWhenExists()
    {
        var locator = new CppBuildToolLocator(CppBuildToolLocator.DefaultPath);

        var result = await locator.LocateAsync(null!);
        result.VsWhereFound.ShouldBeTrue();
        result.InstallationPath.ShouldNotBeNullOrEmpty();
    }
}

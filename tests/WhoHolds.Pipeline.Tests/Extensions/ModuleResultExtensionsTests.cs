using ModularPipelines.Models;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Extensions;

public sealed class ModuleResultExtensionsTests
{
    [Test]
    public void EnsurePublishedShouldThrowIfPublishedBuildIsNull()
    {
        var result = ModuleResult.CreateSuccess<PublishedBuild>(null);

        var ex = Should.Throw<ArgumentNullException>(result.EnsurePublished);
        ex.Message.ShouldContain("publishedBuild");
    }

    [Test]
    public void EnsurePublishedShouldThrowIfPublishedFileDoesNotExist()
    {
        var publishedBuild = new PublishedBuild("wh.exe", AppContext.BaseDirectory, "1.0.0");

        var result = ModuleResult.CreateSuccess(publishedBuild);

        var ex = Should.Throw<FileNotFoundException>(result.EnsurePublished);
        ex.Message.ShouldBe($"Published file not found at path: '{publishedBuild.FilePath}'.");
    }
}

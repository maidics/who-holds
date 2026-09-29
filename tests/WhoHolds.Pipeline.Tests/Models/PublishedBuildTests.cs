using WhoHolds.Pipeline.Models;

namespace WhoHolds.Pipeline.Tests.Models;

public sealed class PublishedBuildTests
{
    [Test]
    [Arguments("artifacts/publish")]
    [Arguments("./publish")]
    [Arguments("")]
    public void ShouldThrowIfDirectoryIsNotAbsolute(string directory)
    {
        var ex = Should.Throw<ArgumentException>(() =>
            new PublishedBuild("wh.exe", directory, "1.0.0")
        );

        ex.ParamName.ShouldBe(nameof(PublishedBuild.Directory));
        ex.Message.ShouldStartWith(
            $"Published build directory must be an absolute path, got: '{directory}'."
        );
    }

    [Test]
    public void FilePathShouldCombineDirectoryAndFileName()
    {
        var directory = Path.Combine(Path.GetTempPath(), "publish");

        var publishedBuild = new PublishedBuild("wh.exe", directory, "1.0.0");

        publishedBuild.Directory.ShouldBe(directory);
        publishedBuild.FilePath.ShouldBe(Path.Combine(directory, "wh.exe"));
    }
}

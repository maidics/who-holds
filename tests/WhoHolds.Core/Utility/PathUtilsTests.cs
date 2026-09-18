using WhoHolds.Core.Common.Models;
using WhoHolds.Core.Tests.TestInfrastructure;
using WhoHolds.Core.Utility;

namespace WhoHolds.Core.Tests.Utility;

internal sealed class PathUtilsTests : PathHandlerTestBase
{
    [Test]
    public void CheckFilePathShouldReturnRuleViolationWhenPathIsADirectory()
    {
        var result = PathUtils.CheckFilePath(_tempDir);
        result.ShouldBeResultedTo(ResultType.RuleViolation, "Given path is a directory.");
    }

    [Test]
    public void CheckFilePathShouldReturnNotFoundIfFileNotFound()
    {
        var nonExisting = Path.Combine(_tempDir, Guid.NewGuid().ToString());

        var result = PathUtils.CheckFilePath(nonExisting);
        result.ShouldBeResultedTo(ResultType.NotFound, "File not found.");
    }

    // UnauthorizedAccessException is left out due to security reasons: TODO: implement it in november

    [Test]
    [Arguments("")]
    [Arguments("file\0.txt")]
    [Arguments("a<b.txt")] // illegal file name characters on Windows
    public void CheckFilePathShouldRuleViolationForInvalidFilePaths(string path)
    {
        var result = PathUtils.CheckFilePath(path);
        result.ShouldBeResultedTo(ResultType.RuleViolation, "Invalid file path.");
    }

    [Test]
    public void ShouldReturnSucceededForValidFilePath()
    {
        var path = CreateTestFile();

        var result = PathUtils.CheckFilePath(path);
        result.ShouldBeResultedTo(ResultType.Success);
    }
}

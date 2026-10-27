using WhoHolds.Cli.Output;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.Output;

public sealed class ResultExtensionTests
{
    [Test]
    public void ShouldReturnNotHeldForSucceededResultWithNoHolderProcesses()
    {
        var result = Result.Success<FileHolderProcess[]>([]);

        result.GetExitCode().ShouldBe(ExitCodes.NotHeld);
    }

    [Test]
    public void ShouldReturnHeldForSucceededResultWithHolderProcesses()
    {
        var holder = new FileHolderProcess(
            122,
            DateTime.UtcNow,
            null,
            null,
            string.Empty,
            [],
            null,
            null,
            false
        );

        var result = Result.Success<FileHolderProcess[]>([holder]);

        result.GetExitCode().ShouldBe(ExitCodes.Held);
    }

    [Test]
    public void ShouldReturnErrorForFailedResult()
    {
        var result = (Result<FileHolderProcess[]>)Result.Failure();
        result.GetExitCode().ShouldBe(ExitCodes.Error);
    }
}

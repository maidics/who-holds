using TUnit.Core;
using WhoHolds.Cli.Output;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.Output;

public sealed class ResultExtensionTests
{
    [Test]
    public void ShouldReturnNotLockedForSucceededResultWithNoHolderProcesses()
    {
        var result = Result.Success<HolderProcess[]>([]);

        result.GetExitCode().ShouldBe(ExitCodes.NotLocked);
    }

    [Test]
    public void ShouldReturnLockedForSucceededResultWithHolderProcesses()
    {
        var holder = new HolderProcess(
            122,
            DateTime.UtcNow,
            null,
            null,
            string.Empty,
            [],
            null,
            false
        );

        var result = Result.Success<HolderProcess[]>([holder]);

        result.GetExitCode().ShouldBe(ExitCodes.Locked);
    }

    [Test]
    public void ShouldReturnErrorForFailedResult()
    {
        var result = (Result<HolderProcess[]>)Result.Failure();
        result.GetExitCode().ShouldBe(ExitCodes.Error);
    }
}

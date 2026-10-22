using WhoHolds.Core.Interop.Services;
using WhoHolds.Core.Tests.TestInfrastructure;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Core.Tests.Interop.Services;

public sealed class ProcessPathResolverTests
{
    // OpenProcess failure requires admin access wizardry
    // GetProcessTimes failure requires the OS to reuse the PID
    // QueryFullProcessImageNameW failure requires the process to exit (ERROR_ACCESS_DENIED),
    // - buffer error in theory is not possible because of using upper limit of path length (ERROR_INSUFFICIENT_BUFFER)

    [Test]
    public async Task ResolveShouldReturnPathForHolderProcess()
    {
        await using var fileSystem = new TestFileSystem(true);
        await fileSystem.InitializeAsync();
        var file = fileSystem.CreateTestFile();
        await using var hold = TestFileSystem.HoldFile(file);

        using var session = new RestartManagerSession([file]);
        session.Start();

        var rmResult = session.GetProcesses(out _);
        rmResult.ShouldBeResultedTo(true);
        rmResult.Value.Length.ShouldBe(1);

        var process = rmResult.Value[0];
        ((int)process.Process.dwProcessId).ShouldBe(Environment.ProcessId);

        var pathResult = ProcessPathResolver.Resolve(process);
        pathResult.ShouldBeResultedTo(true);
        pathResult.Value.ShouldNotBeNullOrWhiteSpace();
    }
}

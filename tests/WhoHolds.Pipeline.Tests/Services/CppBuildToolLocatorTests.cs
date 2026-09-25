using ModularPipelines.Context;
using ModularPipelines.Context.Domains;
using ModularPipelines.Context.Domains.Shell;
using ModularPipelines.Models;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Services;
using WhoHolds.Tests.Shared;

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
        await using var fs = new TestFileSystem();
        await fs.InitializeAsync();

        var file = fs.CreateTestFile();

        var locator = new CppBuildToolLocator(file);

        const string stdout = nameof(stdout);

        var command = ICommandContext.Mock();
        command
            .ExecuteCommandLineTool(Any(), Any(), Any())
            .Returns(
                new CommandResult(
                    commandInput: Repo.VsWhere,
                    workingDirectory: AppContext.BaseDirectory,
                    standardOutput: stdout,
                    standardError: string.Empty,
                    environmentVariables: new Dictionary<string, string?>(),
                    startTime: DateTimeOffset.UnixEpoch,
                    endTime: DateTimeOffset.UnixEpoch,
                    duration: TimeSpan.Zero,
                    exitCode: 0
                )
            );

        var shell = IShellContext.Mock();
        shell.Command.Returns(command.Object);

        var context = IPipelineContext.Mock();
        context.Shell.Returns(shell.Object);

        var result = await locator.LocateAsync(context);
        result.VsWhereFound.ShouldBeTrue();
        result.InstallationPath.ShouldBe(stdout);
    }
}

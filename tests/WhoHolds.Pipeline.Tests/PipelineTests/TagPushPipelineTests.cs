using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModularPipelines;
using ModularPipelines.Context.Domains.Shell;
using ModularPipelines.DotNet.Services;
using ModularPipelines.Enums;
using ModularPipelines.Models;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Services;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Pipeline.Tests.PipelineTests;

public sealed class TagPushPipelineTests
{
    private static IPipeline _pipeline = null!;
    private static TestFileSystem _fileSystem = null!;

    [Before(Class)]
    public static async Task SetUpAsync()
    {
        _fileSystem = new TestFileSystem();
        await _fileSystem.InitializeAsync();

        var builder = ModularPipelines.Pipeline.CreateBuilder();

        var dotnet = IDotNet.Mock();
        dotnet
            .Publish(Any(), Any(), Any())
            .Callback(() => _fileSystem.CreateTestFile(Guid.NewGuid().ToString("N") + ".exe"));
        builder.Services.AddSingleton(dotnet.Object);

        var locator = ICppBuildToolLocator.Mock();
        locator
            .LocateAsync(Any())
            .ReturnsAsync(() =>
                Task.FromResult(new CppBuildToolLookupResult(true, "test-installation-path"))
            );
        builder.Services.AddSingleton(locator.Object);

        builder.Services.AddSingleton<IReleaseVersionResolver, ReleaseVersionResolver>();

        builder.Configuration.Sources.Clear();
        builder.AddJsonConfiguration();

        builder.Configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["GITHUB_REF_TYPE"] = "tag",
                ["GITHUB_REF_NAME"] = "v1.0.0",
                ["Publish:OutputPath"] = _fileSystem.TempDir,
            }
        );

        builder.AddSettings();

        var commandContext = ICommandContext.Mock();
        commandContext
            .ExecuteCommandLineTool(Any(), Any(), Any())
            .ReturnsAsync(() =>
                Task.FromResult(
                    new CommandResult(
                        string.Empty,
                        string.Empty,
                        "1.0.0+test-sha",
                        string.Empty,
                        new Dictionary<string, string?>().AsReadOnly(),
                        DateTimeOffset.UtcNow,
                        DateTimeOffset.UtcNow,
                        TimeSpan.Zero,
                        0
                    )
                )
            );

        builder.Services.AddSingleton(commandContext.Object);

        builder.AddRequirements();
        builder.AddModules();

        _pipeline = await builder.BuildAsync();
    }

    [Test]
    public async Task ShouldReturnSuccess()
    {
        var summary = await _pipeline.RunAsync();
        summary.Status.ShouldBe(Status.Successful);

        var files = Directory.EnumerateFiles(_fileSystem.TempDir).ToList();
        files.Count.ShouldBe(1);
        Path.GetFileName(files[0]).ShouldContain(".exe");
    }

    [After(Class)]
    public static async Task TearDownAsync()
    {
        await _fileSystem.DisposeAsync();
        await _pipeline.DisposeAsync();
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Services;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Settings;
using WhoHolds.Pipeline.Tests.TestInfrastructure;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class PublishModuleTests
{
    private static IOptions<PipelineSettings> CreatePipelineOptions(
        string refType,
        string refName
    ) =>
        Options.Create(
            new PipelineSettings
            {
                GitHubRefType = refType,
                GitHubRefName = refName,
                GitHubTagRef = "tag",
            }
        );

    [Test]
    public void ShouldDependOnTestModule()
    {
        PublishModule.ShouldHaveDependsOnAttribute<PublishModule, TestModule>();
    }

    [Test]
    [Arguments("not-tag", true)]
    [Arguments("tag", false)]
    public async Task ShouldSkipOnNonTagPush(string refType, bool shouldSkip)
    {
        var pipelineOptions = CreatePipelineOptions(refType, string.Empty);
        var publishOptions = Options.Create(
            new PublishSettings
            {
                OutputDirectory = "output-dir",
                Runtime = "runtime",
                ProjectPath = "project",
            }
        );
        var module = new PublishModule(pipelineOptions, publishOptions, null!);
        var config = module.GetConfiguration();
        config.SkipCondition.ShouldNotBeNull();
        var result = await config.SkipCondition.Invoke(null!);
        result.ShouldSkip.ShouldBe(shouldSkip);
    }

    [Test]
    [Arguments(1, 1)]
    [Arguments(0, 0)]
    [Arguments(2, 0)]
    [Arguments(0, 2)]
    public async Task ShouldEraseOutputDirectoryIfContainsFilesOrSubdirectories(
        int fileCount,
        int subdirectoryCount
    )
    {
        await using var fs = new TestFileSystem();
        await fs.InitializeAsync();

        List<string> files = [];
        List<string> subdirectories = [];

        for (int i = 0; i < fileCount; i++)
            files.Add(fs.CreateTestFile());
        for (int i = 0; i < subdirectoryCount; i++)
            subdirectories.Add(fs.CreateSubdirectory());

        var pipelineOptions = CreatePipelineOptions("tag", "ref-name");

        var publishOptions = Options.Create(
            new PublishSettings
            {
                OutputDirectory = fs.TempDir,
                ProjectPath = "project",
                Runtime = "runtime",
            }
        );

        var versionResolver = new FakeReleaseVersionResolver("version");

        var module = new PublishModule(pipelineOptions, publishOptions, versionResolver);

        var logger = new FakeModuleLogger();
        var dotnet = IDotNet.Mock();
        dotnet.Publish(Any(), Any(), Any()).Throws(new NotSupportedException());
        var context = IModuleContext.Create(dotnet, logger);

        await Should.ThrowAsync<NotSupportedException>(() => module.TestExecuteAsync(context));

        var logs = logger.Collector.GetSnapshot();
        var logCount = (fileCount > 0 ? 1 : 0) + (subdirectoryCount > 0 ? 1 : 0);
        logs.Count.ShouldBe(logCount);

        if (fileCount > 0)
            ShouldHaveRemovalLog(
                logs,
                $"Removing {fileCount} files from publish output directory:",
                "FileNames",
                files
            );

        if (subdirectoryCount > 0)
            ShouldHaveRemovalLog(
                logs,
                $"Removing {subdirectoryCount} subdirectories from publish output directory:",
                "SubdirectoryNames",
                subdirectories
            );

        Directory.EnumerateFileSystemEntries(fs.TempDir).ShouldBeEmpty();
    }

    // Enumeration order of the output directory is not guaranteed, so compare names ignoring order.
    private static void ShouldHaveRemovalLog(
        IReadOnlyList<FakeLogRecord> logs,
        string messagePrefix,
        string namesKey,
        IEnumerable<string> expectedPaths
    )
    {
        var log = logs.SingleOrDefault(l => l.Message.StartsWith(messagePrefix, StringComparison.Ordinal));
        log.ShouldNotBeNull();
        log.Level.ShouldBe(LogLevel.Information);

        var names = log.GetStructuredStateValue(namesKey);
        names.ShouldNotBeNull();
        names
            .Split(", ")
            .ShouldBe(expectedPaths.Select(p => Path.GetFileName(p)), ignoreOrder: true);
    }

    [Test]
    [Arguments(0)]
    [Arguments(2)]
    public async Task ShouldThrowIfPublishedExeCountIsNotOne(int exeCount)
    {
        await using var fs = new TestFileSystem();
        await fs.InitializeAsync();

        var pipelineOptions = CreatePipelineOptions("tag", "ref-name");

        var publishOptions = Options.Create(
            new PublishSettings
            {
                OutputDirectory = fs.TempDir,
                ProjectPath = "project",
                Runtime = "runtime",
            }
        );

        var versionResolver = new FakeReleaseVersionResolver("version");

        var module = new PublishModule(pipelineOptions, publishOptions, versionResolver);

        // The output directory is erased before publishing, so the files have to be created by the publish call.
        var dotnet = IDotNet.Mock();
        dotnet
            .Publish(Any(), Any(), Any())
            .Callback(() =>
            {
                for (int i = 0; i < exeCount; i++)
                    fs.CreateTestFile($"test{i}.exe");
            });
        var context = IModuleContext.Create(dotnet);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            module.TestExecuteAsync(context)
        );
        ex.Message.ShouldBe(
            $"Published .exe file count should be exactly one, found: {exeCount}."
        );
    }

    [Test]
    public async Task ShouldRunModule()
    {
        await using var fs = new TestFileSystem();
        await fs.InitializeAsync();

        const string version = "1.0.0";

        var pipelineOptions = CreatePipelineOptions("tag", version);

        var publishOptions = Options.Create(
            new PublishSettings
            {
                OutputDirectory = fs.TempDir,
                Runtime = "runtime",
                ProjectPath = "project",
            }
        );

        var versionResolver = new FakeReleaseVersionResolver(version);

        var module = new PublishModule(pipelineOptions, publishOptions, versionResolver);

        // The output directory is erased before publishing, so the file has to be created by the publish call.
        var dotnet = IDotNet.Mock();
        dotnet.Publish(Any(), Any(), Any()).Callback(() => fs.CreateTestFile("test.exe"));
        var logger = new FakeModuleLogger();
        var context = IModuleContext.Create(dotnet, logger);

        var result = await module.TestExecuteAsync(context);
        result.ShouldNotBeNull();
        result.Directory.ShouldBe(fs.TempDir);
        result.FileName.ShouldBe("test.exe");
        result.Version.ShouldBe(version);

        logger.Collector.Count.ShouldBe(1);
        logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.Collector.LatestRecord.Message.ShouldBe(
            $"Published test.exe file with {version} version."
        );

        dotnet
            .Publish(
                o =>
                    o.NoRestore == false
                    && o.NoBuild == false
                    && o.Nologo == true
                    && o.ProjectSolution == publishOptions.Value.ProjectPath
                    && o.Configuration == pipelineOptions.Value.Configuration
                    && o.Runtime == publishOptions.Value.Runtime
                    && o.Output == publishOptions.Value.OutputDirectory
                    && o.Properties is not null
                    && o.Properties.Count() == 1
                    && o.Properties.FirstOrDefault(p => p.Key == "Version" && p.Value == version)
                        is not null,
                Any(),
                Any()
            )
            .WasCalled(Times.Once);
    }
}

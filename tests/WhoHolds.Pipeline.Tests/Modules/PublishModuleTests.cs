using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;
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
    public async Task EraseOutputDirectoryShouldEraseOutputDirectoryIfContainsFilesOrSubdirectories(
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

        var logger = new FakeModuleLogger();

        PublishModule.EraseOutputDirectory(fs.TempDir, logger);

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
        var log = logs.SingleOrDefault(l =>
            l.Message.StartsWith(messagePrefix, StringComparison.Ordinal)
        );
        log.ShouldNotBeNull();
        log.Level.ShouldBe(LogLevel.Information);

        var names = log.GetStructuredStateValue(namesKey);
        names.ShouldNotBeNull();
        names
            .Split(", ")
            .ShouldBe(expectedPaths.Select(p => Path.GetFileName(p)), ignoreOrder: true);
    }

    [Test]
    public async Task PublishAsyncShouldPublish()
    {
        const string projectPath = nameof(projectPath);
        const string configuration = nameof(configuration);
        const string runtime = nameof(runtime);
        const string outputDirectory = nameof(outputDirectory);
        const string version = nameof(version);
        var dotnet = IDotNet.Mock();

        await PublishModule.PublishAsync(
            projectPath,
            configuration,
            runtime,
            outputDirectory,
            version,
            dotnet,
            CancellationToken.None
        );

        dotnet
            .Publish(
                o =>
                    o.NoRestore == false
                    && o.NoBuild == false
                    && o.Nologo == true
                    && o.ProjectSolution == projectPath
                    && o.Configuration == configuration
                    && o.Runtime == runtime
                    && o.Output == outputDirectory
                    && o.Properties is not null
                    && o.Properties.Count() == 1
                    && o.Properties.FirstOrDefault(p => p.Key == "Version" && p.Value == version)
                        is not null,
                Any(),
                Any()
            )
            .WasCalled(Times.Once);
    }

    [Test]
    [Arguments(0)]
    [Arguments(2)]
    [Arguments(10)]
    public async Task CreatePublishBuildShouldThrowIfOutputDirectoryDoesNotContainExactlyOneExeFile(
        int fileCount
    )
    {
        await using var fs = new TestFileSystem();
        await fs.InitializeAsync();

        for (int i = 0; i < fileCount; i++)
            fs.CreateTestFile(Guid.NewGuid().ToString("N") + ".exe");

        var ex = Should.Throw<InvalidOperationException>(() =>
            PublishModule.CreatePublishedBuild(fs.TempDir, new FakeModuleLogger(), "version")
        );
        ex.Message.ShouldBe(
            $"Published .exe file count should be exactly one, found: {fileCount}."
        );
    }

    [Test]
    public async Task CreatePublishBuildShouldReturnPublishBuild()
    {
        await using var fs = new TestFileSystem();
        await fs.InitializeAsync();

        var exe = fs.CreateTestFile(Guid.NewGuid().ToString("N") + ".exe");
        var exeName = Path.GetFileName(exe);
        var logger = new FakeModuleLogger();
        const string version = nameof(version);

        var publishedBuild = PublishModule.CreatePublishedBuild(fs.TempDir, logger, version);
        publishedBuild.FileName.ShouldBe(exeName);
        publishedBuild.FilePath.ShouldBe(exe);
        publishedBuild.Version.ShouldBe(version);
        publishedBuild.Directory.ShouldBe(fs.TempDir);

        logger.Collector.Count.ShouldBe(1);
        logger.LatestRecord.Level.ShouldBe(LogLevel.Information);
        logger.LatestRecord.Message.ShouldBe($"Published {exeName} file with {version} version.");
    }
}

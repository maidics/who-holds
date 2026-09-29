using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.DotNet.Services;
using ModularPipelines.Logging;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Models;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<TestModule>]
public sealed class PublishModule : Module<PublishedBuild>
{
    private readonly PipelineSettings _pipelineSettings;
    private readonly PublishSettings _publishSettings;
    private readonly IReleaseVersionResolver _versionResolver;

    public PublishModule(
        IOptions<PipelineSettings> pipelineOptions,
        IOptions<PublishSettings> publishOptions,
        IReleaseVersionResolver versionResolver
    )
    {
        _versionResolver = versionResolver;
        _pipelineSettings = pipelineOptions.Value;
        _publishSettings = publishOptions.Value;
    }

    protected override ModuleConfiguration Configure()
    {
        return ModuleConfiguration.Create().WithSkipWhen(_ => !_pipelineSettings.IsTagPush).Build();
    }

    protected override async Task<PublishedBuild?> ExecuteAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        EraseOutputDirectory(_publishSettings.OutputDirectory, context.Logger);

        var version = _versionResolver.Resolve(_pipelineSettings.GitHubRefName);

        await PublishAsync(
            _publishSettings.ProjectPath,
            _pipelineSettings.Configuration,
            _publishSettings.Runtime,
            _publishSettings.OutputDirectory,
            version,
            context.DotNet(),
            cancellationToken
        );

        return CreatePublishedBuild(_publishSettings.OutputDirectory, context.Logger, version);
    }

    public static void EraseOutputDirectory(string outputDirectory, IModuleLogger logger)
    {
        if (!Directory.Exists(outputDirectory))
            return;

        var dir = new DirectoryInfo(outputDirectory);
        var files = dir.EnumerateFiles().ToList();
        var subdirectories = dir.EnumerateDirectories().ToList();

        if (files.Count == 0 && subdirectories.Count == 0)
            return;

        if (files.Count > 0)
        {
            logger.LogInformation(
                "Removing {FileCount} files from publish output directory: {FileNames}.",
                files.Count,
                string.Join(", ", files.Select(f => f.Name))
            );

            foreach (var file in files)
            {
                file.Delete();
            }
        }

        if (subdirectories.Count > 0)
        {
            logger.LogInformation(
                "Removing {SubdirectoryCount} subdirectories from publish output directory: {SubdirectoryNames}",
                subdirectories.Count,
                string.Join(", ", subdirectories.Select(s => s.Name))
            );

            foreach (var subDir in subdirectories)
            {
                subDir.Delete(recursive: true);
            }
        }
    }

    public static async Task PublishAsync(
        string projectPath,
        string configuration,
        string runtime,
        string outputDirectory,
        string version,
        IDotNet dotnet,
        CancellationToken cancellationToken
    )
    {
        var options = new DotNetPublishOptions
        {
            NoRestore = false,
            NoBuild = false,
            Nologo = true,
            ProjectSolution = projectPath,
            Configuration = configuration,
            Runtime = runtime,
            Output = outputDirectory,
            Properties = [new KeyValue("Version", version)],
        };

        await dotnet.Publish(options, cancellationToken: cancellationToken);
    }

    public static PublishedBuild CreatePublishedBuild(
        string outputDirectory,
        IModuleLogger logger,
        string version
    )
    {
        var exeFiles = Directory.GetFiles(outputDirectory, "*.exe");

        if (exeFiles.Length != 1)
            throw new InvalidOperationException(
                $"Published .exe file count should be exactly one, found: {exeFiles.Length}."
            );

        string exeName = Path.GetFileName(exeFiles[0]);

        logger.LogInformation(
            "Published {FileName} file with {Version} version.",
            exeName,
            version
        );

        return new PublishedBuild(exeName, outputDirectory, version);
    }
}

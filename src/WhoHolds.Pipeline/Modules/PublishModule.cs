using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
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
        EraseOutputDirectory(context.Logger);

        var version = _versionResolver.Resolve(_pipelineSettings.GitHubRefName);

        var options = new DotNetPublishOptions
        {
            NoRestore = false,
            NoBuild = false,
            Nologo = true,
            ProjectSolution = _publishSettings.ProjectPath,
            Configuration = _pipelineSettings.Configuration,
            Runtime = _publishSettings.Runtime,
            Output = _publishSettings.OutputDirectory,
            Properties = [new KeyValue("Version", version)],
        };

        await context.DotNet().Publish(options, cancellationToken: cancellationToken);

        var exeFiles = Directory.GetFiles(_publishSettings.OutputDirectory, "*.exe"); // TODO: do testing

        if (exeFiles.Length != 0)
            throw new InvalidOperationException(
                $"Published .exe file count should be exactly one, found: {exeFiles.Length}."
            );

        string exeName = Path.GetFileName(exeFiles[0]);

        context.Logger.LogInformation(
            "Published {FileName} file with {Version} version.",
            exeName,
            version
        );

        return new PublishedBuild(exeName, _publishSettings.OutputDirectory, version);
    }

    private void EraseOutputDirectory(IModuleLogger logger)
    {
        if (!Directory.Exists(_publishSettings.OutputDirectory))
            return;

        var dir = new DirectoryInfo(_publishSettings.OutputDirectory);
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
                subDir.Delete();
            }
        }
    }
}

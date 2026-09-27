using Microsoft.Extensions.Options;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.DotNet.Extensions;
using ModularPipelines.DotNet.Options;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using WhoHolds.Pipeline.Interfaces;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

public sealed class PublishModule : Module<string> // TODO: call in PipelineExtensions, do tests
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

    protected override async Task<string?> ExecuteAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
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

        return _publishSettings.OutputDirectory;
    }
}

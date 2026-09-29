using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Modules;

[DependsOn<PublishModule>]
public sealed class SmokeTestModule(IOptions<PipelineSettings> options) : Module
{
    protected override ModuleConfiguration Configure()
    {
        return ModuleConfiguration.Create().WithSkipWhen(_ => !options.Value.IsTagPush).Build();
    }

    protected override async Task ExecuteModuleAsync(
        IModuleContext context,
        CancellationToken cancellationToken
    )
    {
        var result = await context.GetModule<PublishModule>();
        var publishBuild = result.ValueOrDefault;
        ArgumentNullException.ThrowIfNull(publishBuild);

        if (!File.Exists(publishBuild.FilePath))
            throw new FileNotFoundException(
                $"Published file not found at path: '{publishBuild.FilePath}'."
            );

        var commandResult = await context.Shell.Command.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(publishBuild.FilePath) { Arguments = ["--version"] },
            cancellationToken: cancellationToken
        );

        var trimmed = commandResult.StandardOutput.Trim();
        var parts = trimmed.Split("+");

        if (
            parts.Length != 2
            || !string.Equals(parts[0], publishBuild.Version, StringComparison.Ordinal)
        )
            throw new InvalidOperationException(
                $"Unexpected version output: '{trimmed}'. Expected format: '{{version}}+{{git commit sha}}'."
            );
    }
}

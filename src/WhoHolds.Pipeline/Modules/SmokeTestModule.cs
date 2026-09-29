using Microsoft.Extensions.Options;
using ModularPipelines.Attributes;
using ModularPipelines.Configuration;
using ModularPipelines.Context;
using ModularPipelines.Context.Domains.Shell;
using ModularPipelines.Models;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using WhoHolds.Pipeline.Models;
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
        var publishedBuild = EnsurePublished(await context.GetModule<PublishModule>());

        var result = await ExecuteVersionCommandAsync(
            publishedBuild,
            context.Shell.Command,
            cancellationToken
        );

        ThrowIfVersionOutputInvalid(result.StandardOutput, publishedBuild.Version);
    }

    public static PublishedBuild EnsurePublished(ModuleResult<PublishedBuild?> result)
    {
        var publishedBuild = result.ValueOrDefault;
        ArgumentNullException.ThrowIfNull(publishedBuild);

        if (!File.Exists(publishedBuild.FilePath))
            throw new FileNotFoundException(
                $"Published file not found at path: '{publishedBuild.FilePath}'."
            );

        return publishedBuild;
    }

    public static async Task<CommandResult> ExecuteVersionCommandAsync(
        PublishedBuild publishedBuild,
        ICommandContext context,
        CancellationToken cancellationToken
    )
    {
        // Must be the full path: the bare file name would be looked up in the current directory
        // and on PATH instead of the publish output directory.
        return await context.ExecuteCommandLineTool(
            new GenericCommandLineToolOptions(publishedBuild.FilePath)
            {
                Arguments = ["--version"],
            },
            cancellationToken: cancellationToken
        );
    }

    public static void ThrowIfVersionOutputInvalid(
        ReadOnlySpan<char> stdout,
        string expectedVersion
    )
    {
        var plus = stdout.IndexOf("+");

        if (!(plus >= 0 && stdout[..plus].Equals(expectedVersion, StringComparison.Ordinal)))
            throw new InvalidOperationException(
                $"Unexpected version output: '{stdout.Trim().ToString()}'. "
                    + $"Expected format: '{{version}}+{{git commit sha}}'."
            );
    }
}

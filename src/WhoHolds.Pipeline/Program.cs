using ModularPipelines;
using ModularPipelines.Extensions;
using ModularPipelines.Options;

var builder = Pipeline.CreateBuilder(args);

builder.ConfigurePipelineOptions(options =>
{
    options.PrintLogo = false;
    options.ShowProgressInConsole = true;
    options.ExecutionMode = ExecutionMode.StopOnFirstException;
});

// TODO: add modules

await using var pipeline = await builder.BuildAsync();
await pipeline.RunAsync();

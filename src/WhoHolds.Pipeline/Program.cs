using ModularPipelines;
using WhoHolds.Pipeline.Extensions;

var builder = Pipeline.CreateBuilder(args);

builder
    .ConfigurePipeline("appsettings.json")
    .AddServices()
    .AddGlobalHooks()
    .AddRequirements()
    .AddModules();

await using var pipeline = await builder.BuildAsync();
await pipeline.RunAsync();

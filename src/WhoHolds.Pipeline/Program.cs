using ModularPipelines;
using WhoHolds.Pipeline.Extensions;

var builder = Pipeline.CreateBuilder(args);

builder
    .ConfigurePipeline()
    .AddJsonConfiguration()
    .AddServices()
    .AddGlobalHooks()
    .AddRequirements()
    .AddModules();

await using var pipeline = await builder.BuildAsync();
await pipeline.RunAsync();

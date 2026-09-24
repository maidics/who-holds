using ModularPipelines;
using WhoHolds.Pipeline.Extensions;

var builder = Pipeline.CreateBuilder(args);

builder.AddJsonConfiguration().ConfigurePipeline().AddRequirements().AddModules();

await using var pipeline = await builder.BuildAsync();
await pipeline.RunAsync();

using ModularPipelines;
using WhoHolds.Pipeline.Extensions;

var builder = Pipeline.CreateBuilder(args);

bool isTagPush = builder.Configuration.IsTagPush();

builder.AddGlobalHooks().ConfigurePipeline().AddRequirements(isTagPush).AddModules();

await using var pipeline = await builder.BuildAsync();
await pipeline.RunAsync();

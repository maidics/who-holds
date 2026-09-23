using ModularPipelines;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Settings;

var builder = Pipeline.CreateBuilder(args);

var settings = PipelineSettings.From(builder.Configuration);

builder.ConfigurePipeline().AddRequirements(settings.IsTagPush);

await using var pipeline = await builder.BuildAsync();
await pipeline.RunAsync();

using WhoHolds.Pipeline;

var builder = Pipeline.CreateBuilder(args);

CiPipeline.AddCi(builder);

await using var pipeline = await builder.BuildAsync();
await pipeline.RunAsync();

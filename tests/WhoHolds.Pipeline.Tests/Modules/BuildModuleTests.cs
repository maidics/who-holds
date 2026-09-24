using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using ModularPipelines.Models;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

public sealed class BuildModuleTests
{
    [Test]
    public void ShouldDependOnRestoreModule()
    {
        ModuleTesting<BuildModule>.ShouldDependOn<RestoreModule>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("1.0.0")]
    public async Task ShouldRunDotNetBuild(string? releaseVersion)
    {
        var testing = new ModuleTesting<BuildModule>();

        var summary = await testing.GetSummaryAsync();

        summary.Status.ShouldBe(Status.Successful);

        var expectedOptions = new DotNetBuildOptions
        {
            Nologo = true,
            NoRestore = true,
            ProjectSolution = Repo.Solution,
            Configuration = Repo.Configuration,
            Properties = releaseVersion is null
                ? null
                : [new KeyValue(Repo.DotNetVersionArgumentKey, releaseVersion)],
        };

        testing
            .DotNet.Build(
                o =>
                    o with { Properties = null } == expectedOptions with { Properties = null }
                    && (o.Properties ?? []).SequenceEqual(expectedOptions.Properties ?? []),
                Any(),
                Any()
            )
            .WasCalled(Times.Once);
    }

    [Test]
    public async Task ShouldFailPipelineWhenModuleFails()
    {
        var testing = new ModuleTesting<BuildModule>();

        testing
            .DotNet.Build(Any(), Any(), Any())
            .Throws(new InvalidOperationException("Build failed."));

        var ex = await Should.ThrowAsync<Exception>(testing.GetSummaryAsync);
        ex.Message.ShouldContain(nameof(BuildModule));
    }
}

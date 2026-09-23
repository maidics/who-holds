using ModularPipelines.DotNet.Options;
using ModularPipelines.Enums;
using ModularPipelines.Models;
using WhoHolds.Pipeline.Constants;
using WhoHolds.Pipeline.Modules;
using WhoHolds.Pipeline.Tests.TestInfrastructure;

namespace WhoHolds.Pipeline.Tests.Modules;

[Arguments(null)]
[Arguments("1.0.0")]
public sealed class BuildModuleTests(string? releaseVersion)
    : DotNetModuleTestBase<BuildModule>(_ => new BuildModule(releaseVersion))
{
    [Test]
    public void ShouldDependOnRestoreModule()
    {
        ShouldHaveDependsOnAttribute<RestoreModule>();
    }

    [Test]
    public override async Task ShouldRunModule()
    {
        var summary = await BuildAndRunAsync();

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

        _dotNet
            .Build(
                o =>
                    o with { Properties = null } == expectedOptions with { Properties = null }
                    && (o.Properties ?? []).SequenceEqual(expectedOptions.Properties ?? []),
                Any(),
                Any()
            )
            .WasCalled(Times.Once);
    }

    [Test]
    public override async Task ShouldFailPipelineWhenModuleFails()
    {
        _dotNet.Build(Any(), Any(), Any()).Throws(new InvalidOperationException("Build failed."));

        var ex = await Should.ThrowAsync<Exception>(BuildAndRunAsync);
        ex.Message.ShouldContain(nameof(BuildModule));
    }
}

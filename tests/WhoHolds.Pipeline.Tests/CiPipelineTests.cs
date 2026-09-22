using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ModularPipelines.Modules;
using ModularPipelines.Options;
using ModularPipelines.Requirements;
using WhoHolds.Pipeline.Modules;

namespace WhoHolds.Pipeline.Tests;

public sealed class CiPipelineTests
{
    private static Task<IPipeline> CreateAndAddCi() =>
        ModularPipelines.Pipeline.CreateBuilder().AddCi().BuildAsync();

    [Test]
    public async Task ShouldAddPipelineRequirements()
    {
        var p = await CreateAndAddCi();

        var requirements = p.Services.GetServices<IPipelineRequirement>().ToList();
        requirements.Count.ShouldBe(1);
        requirements.ShouldContain(CiPipeline.WindowsRequirement);
    }

    [Test]
    public async Task ShouldHaveCorrectPipelineOptions()
    {
        var p = await CreateAndAddCi();

        var options = p.Services.GetRequiredService<IOptions<PipelineOptions>>().Value;
        options.PrintLogo.ShouldBeFalse();
        options.ShowProgressInConsole.ShouldBeTrue();
        options.ExecutionMode.ShouldBe(ExecutionMode.StopOnFirstException);
    }

    [Test]
    public async Task ShouldHaveModulesRegistered()
    {
        var p = await CreateAndAddCi();

        var modules = p.Services.GetServices<IModule>().ToList();
        modules.Count.ShouldBe(3);
        modules.FirstOrDefault(m => m is RestoreModule).ShouldNotBeNull();
        modules.FirstOrDefault(m => m is BuildModule).ShouldNotBeNull();
        modules.FirstOrDefault(m => m is TestModule).ShouldNotBeNull();
    }
}

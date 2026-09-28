using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WhoHolds.Pipeline.Extensions;
using WhoHolds.Pipeline.Settings;

namespace WhoHolds.Pipeline.Tests.Extensions;

public sealed class ServiceCollectionExtensionsTests
{
    private readonly IServiceCollection _services = new ServiceCollection();

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    [Test]
    [MethodDataSource(
        nameof(AddCppBuildToolSettingsShouldThrowIfAnyRequiredConfigurationIsMissingSource)
    )]
    public void AddCppBuildToolSettingsShouldThrowIfAnyRequiredConfigurationIsMissing(
        Dictionary<string, string?> values
    )
    {
        var config = BuildConfiguration(values);

        _services.AddCppBuildToolSettings(config);

        using var provider = _services.BuildServiceProvider();

        Should.Throw<OptionsValidationException>(() =>
            provider.GetRequiredService<IStartupValidator>().Validate()
        );
    }

    public static IEnumerable<
        Func<Dictionary<string, string?>>
    > AddCppBuildToolSettingsShouldThrowIfAnyRequiredConfigurationIsMissingSource()
    {
        yield return () =>
            new()
            {
                ["CppBuildTool:VsWhere"] = "vs-where",
                ["CppBuildTool:VsWhereArguments:0"] = "vs-where-args",
            };
        yield return () =>
            new()
            {
                ["CppBuildTool:VsWhereSubdirectory"] = "vs-where-subdirectory",
                ["CppBuildTool:VsWhereArguments:0"] = "vs-where-args",
            };
        yield return () =>
            new()
            {
                ["CppBuildTool:VsWhere"] = "vs-where",
                ["CppBuildTool:VsWhereSubdirectory"] = "vs-where-subdirectory",
            };
    }

    // AddCppBuildToolSetting binds only from appsettings section -> asserting happy path not required

    [Test]
    [MethodDataSource(
        nameof(AddPipelineSettingsShouldThrowIfAnyRequiredConfigurationIsMissingSource)
    )]
    public void AddPipelineSettingsShouldThrowIfAnyRequiredConfigurationIsMissing(
        Dictionary<string, string?> values
    )
    {
        var config = BuildConfiguration(values);

        _services.AddPipelineSettings(config);
        using var provider = _services.BuildServiceProvider();

        Should.Throw<OptionsValidationException>(() =>
            provider.GetRequiredService<IStartupValidator>().Validate()
        );
    }

    public static IEnumerable<
        Func<Dictionary<string, string?>>
    > AddPipelineSettingsShouldThrowIfAnyRequiredConfigurationIsMissingSource()
    {
        yield return () =>
            new()
            {
                ["GITHUB_REF_TYPE"] = "tag",
                ["GITHUB_REF_NAME"] = "ref-name",
                ["Pipeline:GitHubTagRef"] = "tag",
            };
        yield return () =>
            new()
            {
                ["Pipeline:Configuration"] = "config",
                ["GITHUB_REF_NAME"] = "ref-name",
                ["Pipeline:GitHubTagRef"] = "tag",
            };
        yield return () =>
            new()
            {
                ["Pipeline:Configuration"] = "config",
                ["GITHUB_REF_TYPE"] = "tag",
                ["Pipeline:GitHubTagRef"] = "tag",
            };
        yield return () =>
            new()
            {
                ["Pipeline:Configuration"] = "config",
                ["GITHUB_REF_TYPE"] = "tag",
                ["GITHUB_REF_NAME"] = "ref-name",
            };
    }

    [Test]
    public void ShouldAddPipelineSettings()
    {
        var values = new Dictionary<string, string?>
        {
            ["Pipeline:Configuration"] = "config",
            ["GITHUB_REF_TYPE"] = "tag",
            ["GITHUB_REF_NAME"] = "ref-name",
            ["Pipeline:GitHubTagRef"] = "tag",
        };

        var config = BuildConfiguration(values);

        _services.AddPipelineSettings(config);

        var provider = _services.BuildServiceProvider();

        var settings = provider.GetRequiredService<IOptions<PipelineSettings>>().Value;
        settings.Configuration.ShouldBe(values["Pipeline:Configuration"]);
        settings.GitHubRefType.ShouldBe(values["GITHUB_REF_TYPE"]);
        settings.GitHubRefName.ShouldBe(values["GITHUB_REF_NAME"]);
        settings.GitHubTagRef.ShouldBe(values["Pipeline:GitHubTagRef"]);
        settings.Solution.ShouldBe("WhoHolds.slnx");
        settings.IsTagPush.ShouldBeTrue();
    }

    [Test]
    [MethodDataSource(
        nameof(AddPublishSettingsShouldThrowIfAnyRequiredConfigurationIsMissingSource)
    )]
    public void AddPublishSettingsShouldThrowIfAnyRequiredConfigurationIsMissing(
        Dictionary<string, string?> values
    )
    {
        var config = BuildConfiguration(values);

        _services.AddPublishSettings(config);
        using var provider = _services.BuildServiceProvider();

        Should.Throw<OptionsValidationException>(() =>
            provider.GetRequiredService<IStartupValidator>().Validate()
        );
    }

    public static IEnumerable<
        Func<Dictionary<string, string?>>
    > AddPublishSettingsShouldThrowIfAnyRequiredConfigurationIsMissingSource()
    {
        yield return () =>
            new()
            {
                ["Publish:OutputDirectory"] = "output-dir",
                ["Publish:ProjectPath"] = "project-path",
            };
        yield return () =>
            new() { ["Publish:Runtime"] = "runtime", ["Publish:ProjectPath"] = "project-path" };
        yield return () =>
            new() { ["Publish:Runtime"] = "runtime", ["Publish:OutputDirectory"] = "output-dir" };
    }

    // AddPublishSettings binds only from appsettings section -> asserting happy path not required
}

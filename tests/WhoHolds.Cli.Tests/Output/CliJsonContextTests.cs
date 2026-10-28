using System.Reflection;
using System.Text.Json.Serialization;
using WhoHolds.Cli.Output;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.Output;

public sealed class CliJsonContextTests
{
    [Test]
    public void ShouldBeDecoratedWithJsonSourceGenerationOptions()
    {
        var attr =
            typeof(CliJsonContext).GetCustomAttribute<JsonSourceGenerationOptionsAttribute>();
        attr.ShouldNotBeNull();
        attr.WriteIndented.ShouldBeTrue();
        attr.PropertyNamingPolicy.ShouldBe(JsonKnownNamingPolicy.CamelCase);
        attr.Converters.ShouldBe([
            typeof(ResultJsonConverter<FileHolderProcess[]>),
            typeof(ResultJsonConverter<PortHolderProcess[]>),
        ]);
    }

    [Test]
    [Arguments(typeof(Result<FileHolderProcess[]>))]
    [Arguments(typeof(Result<PortHolderProcess[]>))]
    public void ShouldBeDecoratedWithJsonSerializable(Type type)
    {
        CliJsonContext.Default.GetTypeInfo(type).ShouldNotBeNull();
    }
}

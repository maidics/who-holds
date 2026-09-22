using TUnit.Core;
using WhoHolds.Cli.Output;

namespace WhoHolds.Cli.Tests.Output;

public sealed class ResultWriterTests
{
    [Test]
    [Arguments(OutputFormat.Text, typeof(TextResultWriter))]
    [Arguments(OutputFormat.Json, typeof(JsonResultWriter))]
    public void ForMethodShouldReturnCorrectResultWriterForFormat(OutputFormat format, Type type)
    {
        var writer = IResultWriter.For(format);
        writer.GetType().ShouldBe(type);
    }
}

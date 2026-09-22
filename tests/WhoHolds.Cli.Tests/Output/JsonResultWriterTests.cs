using System.Text.Json;
using WhoHolds.Cli.Output;
using WhoHolds.Cli.Tests.TestInfrastructure;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.Output;

public sealed class JsonResultWriterTests // this also covers: ResultJsonConverter
{
    private static readonly JsonResultWriter _writer = new();

    [Test]
    public void ShouldWriteResultWithHolderProcessesForSucceededResultWithHolderProcesses()
    {
        var result = Result.Success(
            new[]
            {
                new HolderProcess(10, DateTime.UtcNow, null, null, string.Empty, [], null, false),
            }
        );

        var stdout = new StringWriter();
        var stderr = new StringWriter();

        _writer.Write(result, stdout, stderr);

        stderr.ToString().ShouldBeEmpty();

        using var doc = JsonDocument.Parse(stdout.ToString());
        var v = doc.ShouldHaveResultedTo(true, JsonValueKind.Array);
        v.GetArrayLength().ShouldBe(1);
        v[0].GetProperty("processId").GetInt32().ShouldBe(result.Value[0].ProcessId);
    }

    [Test]
    public void ShouldWriteResultWithEmptyArrayForSucceededResultAndNoHoldersProcesses()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        _writer.Write(Result.Success<HolderProcess[]>([]), stdout, stderr);

        stderr.ToString().ShouldBeEmpty();

        using var doc = JsonDocument.Parse(stdout.ToString());
        var v = doc.ShouldHaveResultedTo(true, JsonValueKind.Array);
        v.GetArrayLength().ShouldBe(0);
    }

    [Test]
    public void ShouldWriteResultWithEmptyArrayForFailedResult()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var result = (Result<HolderProcess[]>)Result.Failure("test");

        _writer.Write(result, stdout, stderr);

        stderr.ToString().ShouldBeEmpty();

        using var doc = JsonDocument.Parse(stdout.ToString());
        doc.ShouldHaveResultedTo(false, JsonValueKind.Null, result.Errors);
    }
}

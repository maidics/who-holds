using System.Text.Json;
using WhoHolds.Cli.Output;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.Output;

public sealed class JsonResultWriterTests
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
        var root = doc.RootElement;
        root.ValueKind.ShouldBe(JsonValueKind.Object);

        var s = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result.Succeeded)));
        s.ValueKind.ShouldBe(JsonValueKind.True);
        s.GetBoolean().ShouldBeTrue();

        var v = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result<>.Value)));
        v.ValueKind.ShouldBe(JsonValueKind.Array);
        v.GetArrayLength().ShouldBe(1);
        v[0].GetProperty("processId").GetInt32().ShouldBe(result.Value[0].ProcessId);

        var e = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result.Errors)));
        e.ValueKind.ShouldBe(JsonValueKind.Array);
        e.GetArrayLength().ShouldBe(0);
    }

    [Test]
    public void ShouldWriteResultWithEmptyArrayForSucceededResultAndNoHoldersProcesses()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        _writer.Write(Result.Success<HolderProcess[]>([]), stdout, stderr);

        stderr.ToString().ShouldBeEmpty();

        using var doc = JsonDocument.Parse(stdout.ToString());
        var root = doc.RootElement;
        root.ValueKind.ShouldBe(JsonValueKind.Object);

        var s = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result.Succeeded)));
        s.ValueKind.ShouldBe(JsonValueKind.True);
        s.GetBoolean().ShouldBeTrue();

        var v = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result<>.Value)));
        v.ValueKind.ShouldBe(JsonValueKind.Array);
        v.GetArrayLength().ShouldBe(0);

        var e = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result.Errors)));
        e.ValueKind.ShouldBe(JsonValueKind.Array);
        e.GetArrayLength().ShouldBe(0);
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
        var root = doc.RootElement;
        root.ValueKind.ShouldBe(JsonValueKind.Object);

        var s = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result.Succeeded)));
        s.ValueKind.ShouldBe(JsonValueKind.False);
        s.GetBoolean().ShouldBeFalse();

        var v = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result<>.Value)));
        v.ValueKind.ShouldBe(JsonValueKind.Null);

        var e = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result.Errors)));
        e.ValueKind.ShouldBe(JsonValueKind.Array);
        e.GetArrayLength().ShouldBe(1);
        e[0].GetString().ShouldBe(result.Errors[0]);
    }
}

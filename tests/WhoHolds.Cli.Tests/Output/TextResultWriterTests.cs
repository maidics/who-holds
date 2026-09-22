using TUnit.Core;
using WhoHolds.Cli.Output;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.Output;

public sealed class TextResultWriterTests
{
    private readonly TextResultWriter _writer = new();

    private static string[] Lines(StringWriter output) =>
        output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

    [Test]
    public void ShouldWriteEachErrorOnFailure()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var failure = new Result<HolderProcess[]>(false, [], "first error", "second error");

        _writer.Write(failure, stdout, stderr);

        stdout.ToString().ShouldBeEmpty();

        Lines(stderr).ShouldBe(["first error", "second error"]);
    }

    [Test]
    public void ShouldWriteMessageWhenNoHolders()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        _writer.Write(Result.Success<HolderProcess[]>([]), stdout, stderr);

        stderr.ToString().ShouldBeEmpty();

        Lines(stdout).ShouldBe(["No processes are holding the file."]);
    }

    [Test]
    public void ShouldWriteEachHolder()
    {
        var first = new HolderProcess(
            99,
            DateTime.UtcNow,
            null,
            null,
            string.Empty,
            [],
            null,
            false
        );
        var second = new HolderProcess(
            10,
            DateTime.UtcNow,
            null,
            null,
            string.Empty,
            [],
            null,
            false
        );

        var stdout = new StringWriter();
        var stderr = new StringWriter();

        _writer.Write(Result.Success<HolderProcess[]>([first, second]), stdout, stderr);

        stderr.ToString().ShouldBeEmpty();

        var text = stdout.ToString();
        text.ShouldContain(first.ToString());
        text.ShouldContain(second.ToString());
        text.IndexOf(first.ToString()).ShouldBeLessThan(text.IndexOf(second.ToString()));
    }
}

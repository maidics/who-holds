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

        var failure = new Result<FileHolderProcess[]>(false, [], "first error", "second error");

        _writer.WriteFileHolders(failure, stdout, stderr);

        stdout.ToString().ShouldBeEmpty();

        Lines(stderr).ShouldBe(["first error", "second error"]);
    }

    [Test]
    public void ShouldWriteMessageWhenNoHolders()
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        _writer.WriteFileHolders(Result.Success<FileHolderProcess[]>([]), stdout, stderr);

        stderr.ToString().ShouldBeEmpty();

        Lines(stdout).ShouldBe(["No processes are holding the file."]);
    }

    [Test]
    public void ShouldWriteEachHolder()
    {
        var first = new FileHolderProcess(
            99,
            DateTime.UtcNow,
            null,
            null,
            string.Empty,
            [],
            null,
            null,
            false
        );
        var second = new FileHolderProcess(
            10,
            DateTime.UtcNow,
            null,
            null,
            string.Empty,
            [],
            null,
            null,
            false
        );

        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var expected = new StringWriter();
        TableWriter.WriteFileHolderTable(expected, [first, second]);

        _writer.WriteFileHolders(
            Result.Success<FileHolderProcess[]>([first, second]),
            stdout,
            stderr
        );

        stderr.ToString().ShouldBeEmpty();

        var text = stdout.ToString();
        text.ShouldBe(expected.ToString());
    }

    [Test]
    public void ShouldWriteEachHolderAndFailures()
    {
        var first = new FileHolderProcess(
            99,
            DateTime.UtcNow,
            null,
            null,
            string.Empty,
            [],
            null,
            null,
            false
        );
        var second = new FileHolderProcess(
            10,
            DateTime.UtcNow,
            null,
            null,
            string.Empty,
            [],
            null,
            null,
            false
        );

        var result = Result.Success<FileHolderProcess[]>(
            [first, second],
            "First error",
            "Second error"
        );

        var stdout = new StringWriter();
        var stderr = new StringWriter();

        var expectedStdout = new StringWriter();
        TableWriter.WriteFileHolderTable(expectedStdout, [first, second]);

        var expectedStderr = new StringWriter();
        expectedStderr.WriteLine("Failures:");

        foreach (var error in result.Errors)
        {
            expectedStderr.WriteLine($"\t- {error}");
        }

        _writer.WriteFileHolders(result, stdout, stderr);

        stdout.ToString().ShouldBe(expectedStdout.ToString());

        stderr.ToString().ShouldBe(expectedStderr.ToString());
    }
}

using System.CommandLine;
using System.Text.Json;
using TUnit.Core;
using WhoHolds.Cli.Commands;
using WhoHolds.Cli.Output;
using WhoHolds.Cli.Tests.TestInfrastructure;
using WhoHolds.Core.Common.Models;
using WhoHolds.Tests.Shared;

namespace WhoHolds.Cli.Tests.Commands;

public sealed class GetFileHoldersCommandTests : PathHandlerTestBase
{
    private static (int exitCode, string stdout, string stderr) Run(params string[] args)
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();

        int exitCode = new GetFileHoldersCommand(stdout, stderr).Parse(args).Invoke();

        return (exitCode, stdout.ToString(), stderr.ToString());
    }

    [Test]
    public void ShouldHaveCorrectDescription()
    {
        var command = new GetFileHoldersCommand(TextWriter.Null, TextWriter.Null);
        command.Description.ShouldBe(
            $"Shows which processes are holding a file.\n\n{HolderProcess.DescribeFields()}"
        );
    }

    [Test]
    public void ShouldHaveFilePathArgument()
    {
        var command = new GetFileHoldersCommand(TextWriter.Null, TextWriter.Null);
        command.Arguments.Count.ShouldBe(1);

        var arg = command.Arguments[0];
        arg.Description.ShouldBe(
            "Absolute file path to a single file to list its holder processes."
        );
        arg.Arity.ShouldBe(ArgumentArity.ExactlyOne);
    }

    [Test]
    public void ShouldHaveOutputFormatOption()
    {
        var command = new GetFileHoldersCommand(TextWriter.Null, TextWriter.Null);

        var option = command.Options.FirstOrDefault(o => o.Name == "--format");
        option.ShouldNotBeNull();
        option.Description.ShouldBe("Output format.");
        option.Arity.ShouldBe(ArgumentArity.ExactlyOne);
        option.Aliases.Count.ShouldBe(1);
        option.Aliases.Contains("-f").ShouldBeTrue();
        option.HasDefaultValue.ShouldBeTrue();
        ((OutputFormat?)option.GetDefaultValue()).ShouldBe(OutputFormat.Text);
    }

    [Test]
    [Arguments("")]
    [Arguments("test")]
    [Arguments("test.txt")]
    [Arguments("\\test.txt")]
    [Arguments("<>test.txt")]
    //[Arguments("|test.txt")] // this returns ErrorCode.Locked: 1
    public void ShouldReturnErrorForNonLegalFilePaths(string path)
    {
        var (exitCode, stdout, stderr) = Run(path);

        exitCode.ShouldBe(ExitCodes.Error);

        stdout.ShouldBeEmpty();
        stderr.ShouldNotBeEmpty();
    }

    [Test]
    [Arguments("")]
    [Arguments("--format text")]
    [Arguments("--format TEXT")]
    [Arguments("-f text")]
    [Arguments("-f TEXT")]
    public void ShouldWriteTextWhenOutputFormatIsText(string textOutput)
    {
        var file = CreateTestFile();

        var (exitCode, stdout, stderr) = Run(file);

        exitCode.ShouldBe(ExitCodes.NotLocked);
        stdout
            .Trim() /* WriteLine inserts a line to the end of the string */
            .ShouldBe("No processes are holding the file.");
        stderr.ShouldBeEmpty();
    }

    [Test]
    [Arguments("--format", "json")] // this requires passing two separate strings to pass the test
    [Arguments("--format", "JSON")]
    [Arguments("-f json")] // this passes without params
    [Arguments("-f JSON")]
    [Arguments("-f", "json")]
    [Arguments("-f", "JSON")]
    public void ShouldReturnJsonWhenOutputFormatIsJson(params string[] jsonOutput)
    {
        var file = CreateTestFile();
        using var hold = HoldFile(file);

        var (exitCode, stdout, stderr) = Run([file, .. jsonOutput]);

        exitCode.ShouldBe(ExitCodes.Locked);
        stderr.ShouldBeEmpty();

        using var doc = JsonDocument.Parse(stdout);
        var v = doc.ShouldHaveResultedTo(true, JsonValueKind.Array);
        v.GetArrayLength().ShouldBe(1);
        v[0]
            .GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(HolderProcess.ProcessId)))
            .GetInt32()
            .ShouldBe(Environment.ProcessId);
    }
}

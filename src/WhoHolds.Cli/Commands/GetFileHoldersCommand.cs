using WhoHolds.Cli.Output;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Commands;

public sealed class GetFileHoldersCommand : RootCommand
{
    private readonly Argument<string> _filePath = new("filePath")
    {
        Description = "Absolute file path to a single file to list its holder processes.",
        Arity = ArgumentArity.ExactlyOne,
    };

    private readonly Option<OutputFormat> _format = new("--format")
    {
        Description = "Output format.",
        Arity = ArgumentArity.ExactlyOne,
        Aliases = { "-f" },
        DefaultValueFactory = _ => OutputFormat.Text,
    };

    public GetFileHoldersCommand(TextWriter stdout, TextWriter stderr)
        : base($"Shows which processes are holding a file.\n\n{HolderProcess.DescribeFields()}")
    {
        _filePath.AcceptLegalFilePathsOnly();
        Arguments.Add(_filePath);
        Options.Add(_format);

        SetAction(parseResult =>
        {
            var path = parseResult.GetRequiredValue(_filePath)!;
            var format = parseResult.GetValue(_format);

            try
            {
                var result = Core.WhoHolds.File(path);

                var writer = IResultWriter.For(format);

                writer.Write(result, stdout, stderr);

                return result.GetExitCode();
            }
            catch (Exception e)
            {
                stderr.WriteLine(e.Message);

                return ExitCodes.Error;
            }
        });
    }
}

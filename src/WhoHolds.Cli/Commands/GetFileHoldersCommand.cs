using WhoHolds.Cli.Output;

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
        : base("Shows which processes are holding a file.")
    { //TODO: create IResultWriter interface and implement for Text and Json then use it here
        /*
         * internal static class ResultWriters
{
    public static IResultWriter For(OutputFormat format) => format switch
    {
        OutputFormat.Json => new JsonResultWriter(),
        _                 => new TextResultWriter()
    };
}
         */
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
                stdout.WriteLine();
            }
            catch (Exception e)
            {
                stderr.WriteLine(e.Message);
            }
        });
    }
}

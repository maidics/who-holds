using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public sealed class TextResultWriter : IResultWriter
{
    public void Write(Result<FileHolderProcess[]> result, TextWriter stdout, TextWriter stderr)
    {
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                stderr.WriteLine(error);
            }

            return;
        }

        if (result.Value.Length == 0)
        {
            stdout.WriteLine("No processes are holding the file.");
            return;
        }

        TableWriter.Write(stdout, result.Value);

        if (result.Errors.Length > 0)
        {
            stderr.WriteLine("Failures:");

            foreach (var error in result.Errors)
            {
                stderr.WriteLine($"\t- {error}");
            }
        }
    }
}

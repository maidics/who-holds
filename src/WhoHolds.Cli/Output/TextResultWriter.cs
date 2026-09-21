using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public sealed class TextResultWriter : IResultWriter
{
    public void Write(Result<HolderProcess[]> result, TextWriter stdout, TextWriter stderr)
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

        foreach (var holderProcess in result.Value)
        {
            stdout.WriteLine(holderProcess.ToString());
        }
    }
}

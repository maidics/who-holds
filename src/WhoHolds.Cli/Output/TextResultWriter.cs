using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public sealed class TextResultWriter : IResultWriter
{
    public void Write(Result<HolderProcess[]> result, TextWriter output)
    {
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                output.WriteLine(error);
            }

            return;
        }

        if (result.Value.Length == 0)
        {
            output.WriteLine("No processes are holding the file.");
            return;
        }

        foreach (var holderProcess in result.Value)
        {
            output.WriteLine(holderProcess.ToString());
        }
    }
}

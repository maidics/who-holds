using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public sealed class TextResultWriter : IResultWriter
{
    public void WriteFileHolders(
        Result<FileHolderProcess[]> result,
        TextWriter stdout,
        TextWriter stderr
    )
    {
        Write(result, stdout, stderr, "file", TableWriter.WriteFileHolderTable);
    }

    public void WritePortHolders(
        Result<PortHolderProcess[]> result,
        TextWriter stdout,
        TextWriter stderr
    )
    {
        Write(result, stdout, stderr, "port", TableWriter.WritePortHolderTable);
    }

    private void Write<T>(
        Result<T[]> result,
        TextWriter stdout,
        TextWriter stderr,
        string heldObjectName,
        Action<TextWriter, T[]> writerAction
    )
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
            stdout.WriteLine($"No processes are holding the {heldObjectName}.");
            return;
        }

        writerAction.Invoke(stdout, result.Value);

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

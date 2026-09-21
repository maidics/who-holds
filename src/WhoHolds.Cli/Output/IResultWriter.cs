using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public interface IResultWriter
{
    void Write(Result<HolderProcess[]> result, TextWriter stdout, TextWriter stderr);

    public static IResultWriter For(OutputFormat format) =>
        format switch
        {
            OutputFormat.Json => new JsonResultWriter(),
            _ => new TextResultWriter(),
        };
}

using System.Text.Json;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public sealed class JsonResultWriter : IResultWriter
{
    public void Write(Result<FileHolderProcess[]> result, TextWriter stdout, TextWriter stderr)
    {
        stdout.WriteLine(
            JsonSerializer.Serialize(result, CliJsonContext.Default.ResultFileHolderProcessArray)
        );
    }
}

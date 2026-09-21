using System.Text.Json;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public sealed class JsonResultWriter : IResultWriter
{
    public void Write(Result<HolderProcess[]> result, TextWriter stdout, TextWriter stderr)
    {
        stdout.WriteLine(
            JsonSerializer.Serialize(
                result.Succeeded ? result.Value : [],
                CliJsonContext.Default.HolderProcessArray
            )
        );
    }
}

using System.Text.Json;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public sealed class JsonResultWriter : IResultWriter
{
    public void Write(Result<HolderProcess[]> result, TextWriter output)
    {
        output.WriteLine(
            JsonSerializer.Serialize(
                result.Succeeded ? result.Value : [],
                CliJsonContext.Default.HolderProcessArray
            )
        );
    }
}

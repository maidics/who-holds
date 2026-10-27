using System.Text.Json.Serialization;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = [typeof(ResultJsonConverter<FileHolderProcess[]>)]
)]
[JsonSerializable(typeof(Result<FileHolderProcess[]>))]
public sealed partial class CliJsonContext : JsonSerializerContext;

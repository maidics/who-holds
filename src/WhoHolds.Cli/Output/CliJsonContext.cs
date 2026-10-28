using System.Text.Json.Serialization;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = [
        typeof(ResultJsonConverter<FileHolderProcess[]>),
        typeof(ResultJsonConverter<PortHolderProcess[]>),
    ]
)]
[JsonSerializable(typeof(Result<FileHolderProcess[]>))]
[JsonSerializable(typeof(Result<PortHolderProcess[]>))]
public sealed partial class CliJsonContext : JsonSerializerContext;

using System.Net;
using System.Text.Json.Serialization;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    Converters = [
        typeof(ResultJsonConverter<FileHolderProcess[]>),
        typeof(ResultJsonConverter<PortHolderProcess[]>),
        typeof(IPAddressJsonConverter),
    ]
)]
[JsonSerializable(typeof(Result<FileHolderProcess[]>))]
[JsonSerializable(typeof(Result<PortHolderProcess[]>))]
[JsonSerializable(typeof(IPAddress))]
public sealed partial class CliJsonContext : JsonSerializerContext;

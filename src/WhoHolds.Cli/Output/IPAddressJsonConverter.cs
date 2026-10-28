using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WhoHolds.Cli.Output;

public sealed class IPAddressJsonConverter : JsonConverter<IPAddress>
{
    public override IPAddress? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) => throw new NotSupportedException($"{nameof(IPAddress)} is only serialized for CLI output.");

    public override void Write(
        Utf8JsonWriter writer,
        IPAddress value,
        JsonSerializerOptions options
    ) => writer.WriteStringValue(value.ToString());
}

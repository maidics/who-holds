using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Output;

public sealed class ResultJsonConverter<T> : JsonConverter<Result<T>>
{
    public override void Write(
        Utf8JsonWriter writer,
        Result<T> value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();

        writer.WriteBoolean(nameof(Result.Succeeded).ToLower(), value.Succeeded);

        if (value.Succeeded)
        {
            var typeInfo = (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
            JsonSerializer.Serialize(writer, value, typeInfo);
        }
        else
        {
            writer.WriteNullValue();
        }

        writer.WriteStartArray();
        foreach (var error in value.Errors)
            writer.WriteStringValue(error);

        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override Result<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) => throw new NotSupportedException("Result is only serialized for CLI output.");
}

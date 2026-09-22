using System.Text.Json;
using WhoHolds.Core.Common.Models;

namespace WhoHolds.Cli.Tests.TestInfrastructure;

public static class JsonDocumentExtensions
{
    extension(JsonDocument doc)
    {
        public void ShouldHaveResultedTo(bool succeeded, params string[] errors)
        {
            if (succeeded && errors.Length > 0)
                throw new ArgumentException($"Succeeded {nameof(Result)} cannot have errors.");

            var root = doc.RootElement;
            root.ValueKind.ShouldBe(JsonValueKind.Object);

            var s = root.GetProperty(
                JsonNamingPolicy.CamelCase.ConvertName(nameof(Result.Succeeded))
            );
            s.ValueKind.ShouldBe(succeeded ? JsonValueKind.True : JsonValueKind.False);
            s.GetBoolean().ShouldBe(succeeded);

            var e = root.GetProperty(JsonNamingPolicy.CamelCase.ConvertName(nameof(Result.Errors)));
            e.ValueKind.ShouldBe(JsonValueKind.Array);
            e.GetArrayLength().ShouldBe(errors.Length);
        }

        public JsonElement ShouldHaveResultedTo(
            bool succeeded,
            JsonValueKind valueType,
            params string[] errors
        )
        {
            doc.ShouldHaveResultedTo(succeeded, errors);

            var root = doc.RootElement;

            var v = root.GetProperty(
                JsonNamingPolicy.CamelCase.ConvertName(nameof(Result<>.Value))
            );
            v.ValueKind.ShouldBe(valueType);

            return v;
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funzo.Serialization;
public class UnitJsonConverter : JsonConverter<Unit>
{
    public override Unit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Unit.Default;
    }

    public override void Write(Utf8JsonWriter writer, Unit value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Empty);
    }
}

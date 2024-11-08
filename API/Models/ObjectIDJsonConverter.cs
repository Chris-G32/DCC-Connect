using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace API.Models;

public class ObjectIDJsonConverter : JsonConverter<ObjectId>
{
    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ObjectId.Parse(reader.GetString());
    }
}

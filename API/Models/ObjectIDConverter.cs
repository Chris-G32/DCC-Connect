using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
namespace API.Models;
public class ObjectIdConverter : JsonConverter<ObjectId>
{
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (ObjectId.TryParse(value, out var objectId))
        {
            return objectId;
        }
        throw new JsonException($"Invalid ObjectId string: {value}");
    }

    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

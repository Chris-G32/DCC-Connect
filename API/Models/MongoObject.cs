using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace API.Models;

// Note: This object id thingy may cause problems in swagger, not sure yet could need to copy and paste this code to classes if so
public abstract class MongoObject
{
    /// <summary>
    /// Id in database, if already exists
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    [JsonIgnore]
    public ObjectId? Id { get; set; }
}

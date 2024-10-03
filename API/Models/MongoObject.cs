using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.Models;

public interface IMongoObject
{
    public ObjectId? Id { get; set; }
}
public class MongoObject: IMongoObject
{
    /// <summary>
    /// Id in database, if already exists
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    public ObjectId? Id { get; set; }
}

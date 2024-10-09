using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.Models;

public interface IMongoObject
{
    public ObjectId? Id { get; set; }
}
// Note: This object id thingy may cause problems in swagger, not sure yet could need to copy and paste this code to classes if so
public class MongoObject: IMongoObject
{
    /// <summary>
    /// Id in database, if already exists
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    internal ObjectId? Id { get; set; }
}

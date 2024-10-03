using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.Models;

public class ShiftOffer
{
    /// <summary>
    /// Id in database, if already exists
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    internal ObjectId? Id { get; set; }
    public string ShiftId { get; set; }
    public bool? IsTrade { get; set; }
}

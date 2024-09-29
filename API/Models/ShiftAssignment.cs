using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Models;

public class ShiftAssignment
{
    /// <summary>
    /// Id in database, if already exists
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    internal ObjectId? Id { get; set; }
    public string ShiftID { get; set; }
    public string EmployeeID { get; set; }
}

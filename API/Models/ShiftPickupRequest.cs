using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.Models;

public class ShiftPickupRequest
{
    /// <summary>
    /// Id in database, if already exists
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    internal ObjectId? Id { get; set; }
    /// <summary>
    /// The ID of the shift that is being offered to be picked up / traded
    /// </summary>
    public string OfferID { get; set; }
    /// <summary>
    /// ID of the employee who is offering to pick up, not needed if it is a trade request
    /// </summary>
    public string? EmployeeID { get; set; }

    /// <summary>
    /// Null or empty if not a trade request, otherwise the trade ID of the shift being traded for
    /// </summary>
    public string? AssignedShiftToTradeID { get; set; }
}

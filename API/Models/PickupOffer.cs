using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace API.Models;

public class PickupOffer : MongoObject, IRequireManagerApproval
{
    /// <summary>
    /// ID of shift to pickup
    /// </summary>
    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId OpenShiftID { get; internal set; }
    /// <summary>
    /// ID of employee offering to pickup shift
    /// </summary>
    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId EmployeeID { get; internal set; }
    /// <summary>
    /// Status of request for time off. Null means no action taken yet
    /// </summary>
    public bool? IsManagerApproved { get; set; } = null;
}
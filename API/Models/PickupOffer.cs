using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace API.Models;

public class PickupOffer : MongoObject, IRequireManagerApproval
{
    [BsonIgnore]
    public string OpenShiftIDString { get { return OpenShiftID.ToString(); } set { OpenShiftID = ObjectId.Parse(value); } }
    [BsonIgnore]
    public string EmployeeIDString { get { return EmployeeID.ToString(); } set { EmployeeID = ObjectId.Parse(value); } }
    [JsonIgnore]
    /// <summary>
    /// ID of shift to pickup
    /// </summary>
    public ObjectId OpenShiftID { get; internal set; }
    [JsonIgnore]
    /// <summary>
    /// ID of employee offering to pickup shift
    /// </summary>
    public ObjectId EmployeeID { get; internal set; }
    /// <summary>
    /// Status of request for time off. Null means no action taken yet
    /// </summary>
    public bool? IsManagerApproved { get; set; } = null;
}
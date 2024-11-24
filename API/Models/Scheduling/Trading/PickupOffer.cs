using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace API.Models.Scheduling.Trading;

public class PickupOffer : MongoObject, IRequireManagerApproval, IPickupOfferBase<ObjectId>
{
    public PickupOffer() { }
    public PickupOffer (IPickupOfferBase<string> offerBase)
    {
        OpenShiftID = ObjectId.Parse(offerBase.OpenShiftID);
        EmployeeID = ObjectId.Parse(offerBase.EmployeeID);
        IsManagerApproved = null;
    }
    /// <summary>
    /// ID of shift to pickup
    /// </summary>
    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId OpenShiftID { get; set; }
    /// <summary>
    /// ID of employee offering to pickup shift
    /// </summary>
    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId EmployeeID { get; set; }
    /// <summary>
    /// Status of request for time off. Null means no action taken yet
    /// </summary>
    public bool? IsManagerApproved { get; set; } = null;
}
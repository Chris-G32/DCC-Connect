using MongoDB.Bson;

namespace API.Models;

public class PickupOffer : MongoObject, IRequireManagerApproval
{
    /// <summary>
    /// ID of shift to pickup
    /// </summary>
    public ObjectId OpenShiftID { get; set; }
    /// <summary>
    /// ID of employee offering to pickup shift
    /// </summary>
    public ObjectId EmployeeID { get; set; }
    /// <summary>
    /// Status of request for time off. Null means no action taken yet
    /// </summary>
    public bool? IsManagerApproved { get; set; } = null;
}
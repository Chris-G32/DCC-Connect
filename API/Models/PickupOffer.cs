namespace API.Models;

public class PickupOffer:MongoObject{
    /// <summary>
    /// ID of shift to pickup
    /// </summary>
    public string OpenShiftID { get; set; }
    /// <summary>
    /// Status of request for time off. Null means no action taken yet
    /// </summary>
    public bool? IsManagerApproved { get; set; }=null;
}
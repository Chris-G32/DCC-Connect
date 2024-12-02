using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace API.Models.Scheduling.Trading;

public interface IPickupOfferBase<ObjectIDRepresentationType>
{
    /// <summary>
    /// ID of shift to pickup
    /// </summary>
    public ObjectIDRepresentationType OpenShiftID { get; set; }
    /// <summary>
    /// ID of employee offering to pickup shift
    /// </summary>
    public ObjectIDRepresentationType EmployeeID { get; set; }
}

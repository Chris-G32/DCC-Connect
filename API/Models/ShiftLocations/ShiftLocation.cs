
namespace API.Models.ShiftLocations;
public class ShiftLocation : MongoObject
{
    public Address StreetAddress { get; set; }
    /// <summary>
    /// This is just for temporary use. Not sure if we will need this.
    /// </summary>
    public string? PatientName { get; set; }

}

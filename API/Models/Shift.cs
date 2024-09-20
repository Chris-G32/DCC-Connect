using MongoDB.Bson;

namespace API.Models;

public class Shift
{
    /// <summary>
    /// Start time of the shift
    /// </summary>
    public DateTime Start { get; set; }
    /// <summary>
    /// End time of the shift
    /// </summary>
    public DateTime End { get; set; }
    /// <summary>
    /// Address of the home where the shift is taking place
    /// </summary>
    public string Location { get; set; }
    /// <summary>
    /// Role of the employee working the shift
    /// </summary>
    public string Role { get; set; }
}
public class AssignedShift : Shift
{
    /// <summary>
    /// Employee assigned to the shift
    /// </summary>
    public BsonObjectId AssignedEmployeeID { get; set; }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace API.Models.Shifts;

/// <summary>
/// Interface to restrict the string setters of a shift assignment to only be able to get the ObjectId properties.
/// </summary>
public interface IShiftAssignment
{
    ObjectId ShiftID { get; }
    ObjectId? EmployeeID { get; }
}
public class ShiftAssignment: IShiftAssignment
{
    public ShiftAssignment()
    {

    }
    public ShiftAssignment(string shiftId, string employeeId)
    {
        ShiftIDString = shiftId;
        EmployeeIDString = employeeId;
    }
    public ShiftAssignment(ObjectId shiftId, ObjectId? employeeId=null)
    {
        ShiftID = shiftId;
        EmployeeID = employeeId;
    }
    [JsonIgnore]
    public ObjectId ShiftID { get; private set; }
    [JsonIgnore]
    public ObjectId? EmployeeID { get; private set; }
    /// <summary>
    /// String property set from the API. This is used to convert the string to an ObjectId for the ShiftID property.
    /// </summary>
    public string ShiftIDString { get { return ShiftID.ToString(); } set { ShiftID = ObjectId.Parse(value); } }
    /// <summary>
    /// String property set from the API. This is used to convert the string to an ObjectId for the EmployeeID property.
    /// </summary>
    public string? EmployeeIDString { get { return EmployeeID.ToString(); } set { EmployeeID = value == null ? null : ObjectId.Parse(value); } }
}

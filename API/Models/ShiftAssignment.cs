using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace API.Models;

public class ShiftAssignment
{
    public ShiftAssignment()
    {

    }
    public ShiftAssignment(string shiftId,string employeeId)
    {
        ShiftIDString= shiftId;
        EmployeeIDString = employeeId;
    }
    public ShiftAssignment(ObjectId shiftId, ObjectId employeeId)
    {
        ShiftID = shiftId;
        EmployeeID = employeeId;
    }
    [JsonIgnore]
    public ObjectId ShiftID { get; internal set; }
    [JsonIgnore]
    public ObjectId? EmployeeID { get; internal set; }
    [BsonIgnore]
    public string ShiftIDString { get{ return ShiftID.ToString(); } set { ShiftID = ObjectId.Parse(value); } }
    [BsonIgnore]
    public string? EmployeeIDString { get { return EmployeeID.ToString(); } set { EmployeeID = ObjectId.Parse(value); } }
}

using MongoDB.Bson;

namespace API.Models;

public class ShiftAssignment
{
    public required ObjectId ShiftID { get; set; }
    public ObjectId? EmployeeID { get; set; }
}

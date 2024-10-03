using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Models;

public class ShiftAssignment:MongoObject
{
    public string ShiftID { get; set; }
    public string EmployeeID { get; set; }
}

using MongoDB.Bson;

namespace API.Models;

public class ShiftAssignment
{
    public string Shift { get; set; }
    public string Employee { get; set; }
}

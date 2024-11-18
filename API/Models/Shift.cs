using API.Constants;
using API.Models.QueryOptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models;

public class Shift : MongoObject
{
    public Shift() { }
    private const double MAX_SHIFT_LENGTH_HRS = 16;// According to derron. May need updated.
    private TimeRange _shiftPeriod;
    /// <summary>
    /// When the shift is taking place
    /// </summary>
    public TimeRange ShiftPeriod
    {
        get { return _shiftPeriod; }
        set
        {
            if (value.Duration().TotalHours < 0)
            {
                throw new ArgumentException($"A shift may not start after it ends.");

            }
            if (value.Duration().TotalHours > MAX_SHIFT_LENGTH_HRS)
            {
                throw new ArgumentException($"Shift length is too long, please be sure shifts are less than {MAX_SHIFT_LENGTH_HRS} hours");
            }
            _shiftPeriod = value;
        }
    }

    /// <summary>
    /// Address of the home where the shift is taking place
    /// </summary>
    [Length(0,70)]
    public string Location { get; set; }
    private string _requiredRole;
    /// <summary>
    /// Role of the employee working the shift
    /// </summary>
    public string RequiredRole
    {
        get { return _requiredRole; }
        set
        {
            if (!RoleConstants.ValidRoles.Contains(value))
            {
                throw new InvalidDataException($"\"{value}\" is not a valid role.");
            }
            _requiredRole = value;
        }
    }
    /// <summary>
    /// Employee assigned to the shift, null or empty if none.
    /// </summary>
    [BsonIgnoreIfNull]
    public ObjectId? EmployeeID { get; set; }
}
using API.Constants;
using API.Models.QueryOptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models.Shifts;
public class Shift : MongoObject
{
    public Shift() { }
    public Shift(ShiftCreationInfo creationInfo)
    {
        ShiftPeriod = creationInfo.ShiftPeriod;
        Location = ObjectId.Parse(creationInfo.LocationID);
        RequiredRole = creationInfo.RequiredRole;
    }
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
    /// Object id of the location the shift is at
    /// </summary>
    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId Location { get; set; }

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
    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId? EmployeeID { get; set; }

    public string Summary() {return $"Start: {ShiftPeriod.Start.ToLongDateString()}\nEnd: {ShiftPeriod.End.ToLongDateString()}"; } 
}
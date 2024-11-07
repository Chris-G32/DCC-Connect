using API.Constants;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;

namespace API.Models;

public class TimeOffRequest:MongoObject,IRequireManagerApproval{
    /// <summary>
    /// Start time of the off request
    /// </summary>
    public DateTime Start { get; set; }
    /// <summary>
    /// End time of the off request
    /// </summary>
    public DateTime End { get; set; }
    /// <summary>
    /// ID of employee requesting off.
    /// </summary>
    public ObjectId EmployeeID { get; set; }
    /// <inheritdoc/>
    public bool? IsManagerApproved { get; set; }=null;
    
}
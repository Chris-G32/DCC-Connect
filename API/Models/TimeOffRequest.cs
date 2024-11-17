using API.Constants;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace API.Models;

/// <summary>
/// Received at API endpoint to request time off.
/// </summary>
public class TimeOffRequestExternal
{
    /// <summary>
    /// The span of time requested off
    /// </summary>
    public TimeRange TimeOffTimeSpan { get; set; }
    public string EmployeeID { get; set; }
}
public class TimeOffRequest:MongoObject,IRequireManagerApproval{
    public TimeOffRequest() { }
    public TimeOffRequest(TimeOffRequestExternal request)
    {
        TimeOffTimeSpan = request.TimeOffTimeSpan;
        EmployeeID = ObjectId.Parse(request.EmployeeID);
    }
    /// <summary>
    /// The span of time requested off
    /// </summary>
    public TimeRange TimeOffTimeSpan { get; set; }
    /// <summary>
    /// ID of employee requesting off.
    /// </summary>
    public ObjectId EmployeeID { get; set; }
    /// <inheritdoc/>
    public bool? IsManagerApproved { get; set; }=null;
    
}
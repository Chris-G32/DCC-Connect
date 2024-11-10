using API.Constants;
using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

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
    [BsonIgnore]
    public string EmployeeIDString { get { return EmployeeID.ToString(); } set { EmployeeID=ObjectId.Parse(value); } }
    [JsonIgnore]
    /// <summary>
    /// ID of employee requesting off.
    /// </summary>
    public ObjectId EmployeeID { get; set; }
    /// <inheritdoc/>
    public bool? IsManagerApproved { get; set; }=null;
    
}
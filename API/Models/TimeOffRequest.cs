namespace API.Models;

public class TimeOffRequest:MongoObject{
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
    public string EmployeeID { get; set; }
    /// <summary>
    /// Status of request for time off. Null means no action taken yet
    /// </summary>
    public bool? IsManagerApproved { get; set; }=null;

}
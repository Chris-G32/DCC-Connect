using MongoDB.Bson;

namespace API.Models.QueryOptions;


public interface OpenShiftQueryOptions
{
    /// <summary>
    /// Only get shifts that start within this range
    /// </summary>
    TimeRange? TimeFilter { get; set; }
}
public interface ShiftQueryOptions : OpenShiftQueryOptions
{
    /// <summary>
    /// Only get shifts assigned to this employee
    /// </summary>
    ObjectId? EmployeeIDFilter { get; set; }
}

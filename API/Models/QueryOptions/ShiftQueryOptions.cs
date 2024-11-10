using MongoDB.Bson;

namespace API.Models.QueryOptions;


public interface IOpenShiftQueryOptions:IHaveTimeFilterOption;

public interface ShiftQueryOptions : IOpenShiftQueryOptions
{
    /// <summary>
    /// Only get shifts assigned to this employee
    /// </summary>
    ObjectId? EmployeeIDFilter { get; set; }
}
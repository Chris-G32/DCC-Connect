using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Serialization;

namespace API.Models.QueryOptions;

public interface IEmployeeQueryOptions:IHaveTimeFilterOption
{
    public string? EmployeeRole { get; set; }
}
public class EmployeeQueryOptions : IEmployeeQueryOptions
{
    /// <summary>
    /// This filter's for employees that are available during a time range. This is intended to be used to provide employees that could be scheduled for a specific shift, or day.
    /// </summary>
    public TimeRange? TimeFilter { get; set; }
    /// <summary>
    /// This filter's for employees that have a specific role.
    /// </summary>
    public string? EmployeeRole { get; set; }

}
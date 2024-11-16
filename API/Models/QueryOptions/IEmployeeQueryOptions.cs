using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Serialization;

namespace API.Models.QueryOptions;

public interface IEmployeeQueryOptions:IHaveIdFilterOption,IHaveTimeFilterOption
{
}
public class EmployeeQueryOptions : IEmployeeQueryOptions
{
    [JsonIgnore]
    public ObjectId? UniqueID { get; set; }
    public string? UniqueIDFilterString { get { return UniqueID.ToString(); } set { UniqueID = ObjectId.Parse(value); } }
    /// <summary>
    /// This filter's for employees that are available during a time range. This is intended to be used to provide employees that could be scheduled for a specific shift, or day.
    /// </summary>
    public TimeRange? TimeFilter { get; set; }

}
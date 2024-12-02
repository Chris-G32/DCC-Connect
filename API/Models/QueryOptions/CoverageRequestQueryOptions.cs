using API.Models.Scheduling.Coverage;
using MongoDB.Bson;

namespace API.Models.QueryOptions;

public class CoverageRequestQueryOptions : ShiftQueryOptions
{
    /// <summary>
    /// Get coverage requests that can be picked up
    /// </summary>
    public bool? PickupsOnly { get; set; }
    /// <summary>
    /// Get coverage requests that can be traded
    /// </summary>
    public bool? TradesOnly { get; set; }
    public bool IsValidQuery()
    {
        return !(PickupsOnly == true && TradesOnly == true);
    }
}

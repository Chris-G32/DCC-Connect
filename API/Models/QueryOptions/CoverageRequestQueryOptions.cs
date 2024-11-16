using MongoDB.Bson;

namespace API.Models.QueryOptions;

public class CoverageRequestQueryOptions : ShiftQueryOptions
{
    public CoverageOptions? CoverageType { get; set; }
}

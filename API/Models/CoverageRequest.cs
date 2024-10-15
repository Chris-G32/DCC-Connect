namespace API.Models;

public enum CoverageOptions
{
    PickupOnly,
    TradeOnly,
    PickupOrTrade
}

public class CoverageRequest : MongoObject
{
    /// <summary>
    /// Start time of the off request
    /// </summary>
    public string ShiftID { get; set; }
    /// <summary>
    /// Type of coverage wanted.
    /// </summary>
    public CoverageOptions CoverageType { get; set; }
    /// <summary>
    /// Optional note to leave with coverage request.
    /// </summary>
    public string? Note { get; set; }
}
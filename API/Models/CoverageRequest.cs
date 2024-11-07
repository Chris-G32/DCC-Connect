using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

public enum CoverageOptions
{
    PickupOnly = 0,
    TradeOnly = 1,
    PickupOrTrade = 2
}

public class CoverageRequest : MongoObject
{
    public bool CanPickup()
    {
        return CoverageType == CoverageOptions.PickupOnly || CoverageType == CoverageOptions.PickupOrTrade;
    }
    public bool CanTrade()
    {
        return CoverageType == CoverageOptions.TradeOnly || CoverageType == CoverageOptions.PickupOrTrade;
    }
    /// <summary>
    /// Start time of the off request
    /// </summary>
    public ObjectId ShiftID { get; set; }
    /// <summary>
    /// Type of coverage wanted.
    /// </summary>
    public CoverageOptions CoverageType { get; set; }
    /// <summary>
    /// Optional note to leave with coverage request.
    /// </summary>
    [Length(0,300)]
    public string? Note { get; set; }
    

}
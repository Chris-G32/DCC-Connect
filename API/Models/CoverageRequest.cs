using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

public enum CoverageOptions
{
    PickupOnly = 0,
    TradeOnly = 1,
    PickupOrTrade = 2
}
public interface ICoverageRequestBase<ObjectIDRepresentationType>
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
    public ObjectIDRepresentationType ShiftID { get; set; }
    /// <summary>
    /// Type of coverage wanted.
    /// </summary>
    public CoverageOptions CoverageType { get; set; }
    /// <summary>
    /// Optional note to leave with coverage request.
    /// </summary>
    [Length(0, 300)]
    public string? Note { get; set; }
}
public class CoverageRequestBase<T> : MongoObject, ICoverageRequestBase<T>
{
    public CoverageRequestBase()
    {

    }
    protected CoverageRequestBase(CoverageOptions coverageType, string? note, T shiftId)
    {
        CoverageType = coverageType;
        Note = note;
        ShiftID = shiftId;
    }
    static CoverageRequestBase()
    {
        if (typeof(T) != typeof(string) && typeof(T) != typeof(ObjectId))
        {
            throw new ArgumentException("T must be either string or ObjectId");
        }
    }
    public T ShiftID { get; set; }
    public CoverageOptions CoverageType { get; set; }
    [Length(0, 300)]
    public string? Note { get; set; }
}
public class CoverageRequest : CoverageRequestBase<ObjectId>
{

    public CoverageRequest() { }
    public CoverageRequest(CoverageRequestBase<string> coverageRequest) : base(coverageRequest.CoverageType, coverageRequest.Note, ObjectId.Parse(coverageRequest.ShiftID)) { }

    public bool CanPickup()
    {
        return CoverageType == CoverageOptions.PickupOnly || CoverageType == CoverageOptions.PickupOrTrade;
    }
    public bool CanTrade()
    {
        return CoverageType == CoverageOptions.TradeOnly || CoverageType == CoverageOptions.PickupOrTrade;
    }

}
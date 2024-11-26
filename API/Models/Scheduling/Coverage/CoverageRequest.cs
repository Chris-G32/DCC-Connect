using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models.Scheduling.Coverage;



public class CoverageRequest : MongoObject, ICoverageRequestBase<ObjectId>
{
    public CoverageRequest() { }
    public CoverageRequest(ObjectId shiftId, CoverageOptions coverageType, string? note)
    {
        CoverageType = coverageType;
        Note = note;
        ShiftID = shiftId;
    }
    public CoverageRequest(ICoverageRequestBase<string> coverageRequest)
        : this(ObjectId.Parse(coverageRequest.ShiftID), coverageRequest.CoverageType, coverageRequest.Note) { }

    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId ShiftID { get; set; }
    public CoverageOptions CoverageType { get; set; }
    public string? Note { get; set; }
    public bool CanPickup()
    {
        return CoverageType == CoverageOptions.PickupOnly || CoverageType == CoverageOptions.PickupOrTrade;
    }
    public bool CanTrade()
    {
        return CoverageType == CoverageOptions.TradeOnly || CoverageType == CoverageOptions.PickupOrTrade;
    }

}
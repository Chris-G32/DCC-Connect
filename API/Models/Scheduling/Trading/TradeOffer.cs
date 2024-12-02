using API.Constants;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace API.Models.Scheduling.Trading;


/// <summary>
/// This represents a trade offer in response to a coverage request in the database.
/// </summary>
public class TradeOffer : MongoObject, IRequireManagerApproval, IRequireEmployeeApproval,ITradeOfferBase<ObjectId>
{
    public TradeOffer() { }
    public TradeOffer(ITradeOfferBase<string> info)
    {
        CoverageRequestID = ObjectId.Parse(info.CoverageRequestID);
        ShiftOfferedID = ObjectId.Parse(info.ShiftOfferedID);
        IsEmployeeApproved = null;
        IsManagerApproved = null;
    }
    /// <summary>
    /// ID of coverage request the offer is for
    /// </summary>
    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId CoverageRequestID { get; set; }
    /// <summary>
    /// ID of the shift being offered for trade.
    /// </summary>
    [JsonConverter(typeof(ObjectIdConverter))]
    public ObjectId ShiftOfferedID { get; set; }
    /// <summary>
    /// Whether the employee associated with the coverage request has approved it or not. Null means no action taken yet
    /// </summary>
    public bool? IsEmployeeApproved { get; set; } = null;
    /// <inheritdoc/>
    public bool? IsManagerApproved { get; set; } = null;

}
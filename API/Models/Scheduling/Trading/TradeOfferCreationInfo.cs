using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace API.Models.Scheduling.Trading;

public class TradeOfferCreationInfo:ITradeOfferBase<string>
{
    /// <summary>
    /// ID of coverage request the offer is for
    /// </summary>
    public string CoverageRequestID { get; set; }
    /// <summary>
    /// ID of the shift being offered for trade.
    /// </summary>
    public string ShiftOfferedID { get; set; }
}

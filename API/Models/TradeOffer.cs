using API.Constants;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace API.Models;



public class TradeOffer : MongoObject, IRequireManagerApproval, IRequireEmployeeApproval
{
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
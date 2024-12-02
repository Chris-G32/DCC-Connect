using System.Text.Json.Serialization;

namespace API.Models.Scheduling.Trading;

public interface ITradeOfferBase<ObjectIDRepresentationType>
{
    /// <summary>
    /// ID of coverage request the offer is for
    /// </summary>
    public ObjectIDRepresentationType CoverageRequestID { get; set; }
    /// <summary>
    /// ID of the shift being offered for trade.
    /// </summary>
    public ObjectIDRepresentationType ShiftOfferedID { get; set; }
}

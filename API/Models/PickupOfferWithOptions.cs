using API.Services;

namespace API.Models;

public class PickupOfferWithOptions : PickupOffer
{
    public OpenShiftQueryOptions Options { get; set; }
}

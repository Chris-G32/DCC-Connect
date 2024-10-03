using Amazon.Runtime.Internal;
using API.Models;
using MongoDB.Driver;

namespace API.Services;

public interface IShiftTrader
{
    /// <summary>
    /// Offers a shift to be either picked up or traded for by another employee.
    /// </summary>
    /// <param name="offer"></param>
    public void OfferUpShift(ShiftOffer offer);
    /// <summary> 
    /// Offers a trade to another employee for a shift they requested traded/covered.
    /// </summary>
    /// <param name="request"></param>
    public void OfferTrade(ShiftPickupRequest request);
}
public class ShiftTrader(ICollectionsProvider collectionProvider) : IShiftTrader
{
    private readonly ICollectionsProvider _collectionsProvider = collectionProvider;
    private void PickupShift(ShiftPickupRequest request)
    {
        if (!string.IsNullOrEmpty(request.AssignedShiftToTradeID))
        {
            throw new Exception("Trade provided for a pickup request.");
        }
        if (string.IsNullOrEmpty(request.EmployeeID))
        {
            throw new Exception("Please provide an employee ID to pickup a shift.");
        }
        _collectionsProvider.TradeRequests.InsertOne(request);
    }
    private void TradeShift(ShiftPickupRequest request)
    {
        if (string.IsNullOrEmpty(request.AssignedShiftToTradeID))
        {
            throw new Exception("No trade provided for a trade request.");
        }
        if (!string.IsNullOrEmpty(request.EmployeeID))
        {
            throw new Exception("Please remove employee ID from trade request. It is redundant.");
        }
        _collectionsProvider.TradeRequests.InsertOne(request);
    }
    public void OfferTrade(ShiftPickupRequest request)
    {
        var offeredUpShift = _collectionsProvider.OfferedUpShifts.Find(offer => offer.Id.ToString() == request.OfferID).FirstOrDefault();
        if (offeredUpShift == null)
        {
            throw new Exception("No shift with this id exists to pickup/trade for.");
        }
        if (offeredUpShift.IsTrade == false) 
        {
            PickupShift(request);
        }
        else
        {
            TradeShift(request);
        }
    }
    public void OfferUpShift(ShiftOffer offer)
    {
        _collectionsProvider.OfferedUpShifts.InsertOne(offer);
    }
    public void RequestApproval() { }
    public void Approve(ShiftPickupRequest request) { }
    public void CancelOfferUpShift(string offerID)
    {
        // Remove offers for this shift, consider notifying those that said they would pick it up
        _collectionsProvider.TradeRequests.DeleteMany(request => request.OfferID.ToString() == offerID);
        _collectionsProvider.OfferedUpShifts.FindOneAndDelete(offerID);
    }
}
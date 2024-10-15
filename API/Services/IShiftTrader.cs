using Amazon.Runtime.Internal;
using API.Constants;
using API.Models;
using API.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace API.Services;

public interface IShiftTrader
{
    /// <summary>
    /// Offers a shift to be either picked up or traded for by another employee.
    /// </summary>
    /// <param name="offer"></param>
    public void RequestCoverage(CoverageRequest request);
    /// <summary> 
    /// Offers a trade to another employee for a shift they requested traded/covered.
    /// </summary>
    /// <param name="request"></param>
    public void OfferTrade(TradeOffer offer);
    /// <summary> 
    /// Offers to pickup a shift
    /// </summary>
    /// <param name="request"></param>
    public void PickupShift(PickupOfferWithOptions offer);
}
public class ShiftTrader(ICollectionsProvider collectionProvider, IShiftRetriever shiftRetriever) : IShiftTrader
{
    private readonly ICollectionsProvider _collectionsProvider = collectionProvider;
    private readonly IShiftRetriever _shiftRetriever = shiftRetriever;
    public void RequestCoverage(CoverageRequest request)
    {
        _collectionsProvider.CoverageRequests.InsertOne(request);
    }
    /// <summary>
    /// Offers a shift to trade for a coverage request. The coverage request must be for a shift that is up for trade.
    /// </summary>
    /// <param name="offer"> Offer to make</param>
    /// <exception cref="Exception"> Either object does not exist or the shift is not up for trade.</exception>
    public void OfferTrade(TradeOffer offer)
    {
        // Assert shift offered exists.
        DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, offer.ShiftOfferedID);

        var coverage = _collectionsProvider.CoverageRequests.
            Find(coverage => coverage.Id.ToString() == offer.CoverageRequestID).FirstOrDefault()
            ?? throw new Exception(ErrorUtils.FormatObjectDoesNotExistErrorString(offer.CoverageRequestID, CollectionConstants.CoverageRequestsCollection));

        switch (coverage.CoverageType)
        {
            case CoverageOptions.TradeOnly:
            case CoverageOptions.PickupOrTrade:
                _collectionsProvider.TradeOffers.InsertOne(offer);
                break;
            default:
                throw new Exception("Cannot offer a trade for a shift that is only up for trade.");
        }
    }

    public void PickupShift(PickupOfferWithOptions offer)
    {
        DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Employees, offer.EmployeeID);
        DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, offer.OpenShiftID);

        //Note: Potential performance boost here would be to filter out by the date of the shift being requested to be picked up.
        // Verify shift is open for pickup
        if (!_shiftRetriever.GetOpenShiftIDs(offer.Options).Any(id => id == offer.OpenShiftID))
        {
            throw new Exception("Cannot pick up a shift that is not open.");
        }
        var openShift = _collectionsProvider.Shifts.Find(shift => shift.Id.ToString() == offer.OpenShiftID).FirstOrDefault(); // Assert shift exists (should never be null because of the above check
        // Verify employee has not already requested to pick up the shift
        if (_collectionsProvider.PickupOffers.CountDocuments(existingOffer => existingOffer.OpenShiftID == offer.OpenShiftID && existingOffer.EmployeeID == offer.EmployeeID) > 0)
        {
            throw new Exception("Duplicate pickup request.");
        }
        // Verify employee is not scheduled for another shift at that time
        // Note this may be a good candidate to create a helper function or service
        if (_collectionsProvider.Shifts.CountDocuments(shift =>
            shift.EmployeeID == offer.EmployeeID &&
            shift.Start < openShift.End &&    // Shift starts before openShift ends
            shift.End > openShift.Start) > 0)  // Shift ends after openShift starts
        {
            throw new Exception("Cannot pick up a shift that overlaps with another shift.");
        }
        _collectionsProvider.PickupOffers.InsertOne(offer);
    }
}
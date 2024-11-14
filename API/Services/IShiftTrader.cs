using Amazon.Runtime.Internal;
using API.Constants;
using API.Models;
using API.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using API.Services.Retrievers;
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
    public void PickupShift(PickupOffer offer);
    public void ApproveTrade(string tradeOfferID, bool isManager = false);
    public void ApprovePickup(string tradeOfferID);
    public void DenyTrade(string tradeOfferID, bool isManager = false);
    public void DenyPickup(string tradeOfferID);

}
public class ShiftTrader(ILogger<ShiftTrader> logger, IEntityRetriever entityRetriever, IShiftScheduler scheduler, IShiftRetriever shiftRetriever, IAvailabiltyService availabilityService) : IShiftTrader
{
    private readonly ILogger<ShiftTrader> _logger=logger;
    private readonly IEntityRetriever _entityRetriever = entityRetriever;
    private readonly ICollectionsProvider _collectionsProvider = entityRetriever.CollectionsProvider;
    private readonly IShiftRetriever _shiftRetriever = shiftRetriever;
    private readonly IAvailabiltyService _availabilityService = availabilityService;
    private readonly IShiftScheduler _scheduler = scheduler;

    private void ExecuteTrade(TradeOffer tradeOffer)
    {
        
        // Unassign the shift from the coverage request 
        var coverageRequest = _entityRetriever.GetEntityOrThrow(_collectionsProvider.CoverageRequests, tradeOffer.CoverageRequestID); ;
        var coverageRequestShift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, coverageRequest.ShiftID);

        _scheduler.UnassignShift(coverageRequest.ShiftID);

        //Unassign the shift offered for the trade
        var offeredShift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, tradeOffer.ShiftOfferedID);
        _scheduler.UnassignShift(tradeOffer.ShiftOfferedID);

        //Assign shift offered for trade to the employee requesting coverage.
        _scheduler.AssignShift(new ShiftAssignment
        {
            EmployeeID = coverageRequestShift.EmployeeID,
            ShiftID = tradeOffer.ShiftOfferedID
        });
        //Assign shift requesting coverage to employee offering trade.
        _scheduler.AssignShift(new ShiftAssignment
        {
            EmployeeID = offeredShift.EmployeeID,
            ShiftID = coverageRequest.ShiftID
        });
        _logger.LogInformation("Trade executed.");
    }
    public void RequestCoverage(CoverageRequest request)
    {
        var shift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, request.ShiftID);
        if (shift.ShiftPeriod.Start < DateTime.Now)
        {
            throw new Exception("Shift already started. Cannot request coverage.");
        }
        // Authenticate employee
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
        var offeredShift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, offer.ShiftOfferedID);
        var coverageReq = _entityRetriever.GetEntityOrThrow(_collectionsProvider.CoverageRequests, offer.CoverageRequestID);
        if (offeredShift.ShiftPeriod.Start < DateTime.Now)
        {
            throw new Exception("Cannot offer a trade for a shift that has already started.");
        }
        if (coverageReq.CanTrade())
        {
            _collectionsProvider.TradeOffers.InsertOne(offer);
            //TODO: Notify employee
            return;
        }
        throw new Exception("Cannot offer a trade for a shift that is only up for trade.");
    }

    public void PickupShift(PickupOffer offer) //Logic breakdown. Validate that the pertinent ID's exist. Then validate shift is actually open.
    {
        var openShift = _entityRetriever.GetEntityOrThrow(_collectionsProvider.Shifts, offer.OpenShiftID);
        if (!_availabilityService.IsEmployeeSchedulableForShift(offer.EmployeeID, offer.OpenShiftID))
        {
            throw new Exception("Shift cannot be picked up.");
        }

        // Verify employee has not already requested to pick up the shift
        if (_collectionsProvider.PickupOffers.CountDocuments(existingOffer => existingOffer.OpenShiftID == offer.OpenShiftID && existingOffer.EmployeeID == offer.EmployeeID) > 0)
        {
            throw new Exception("Duplicate pickup request.");
        }
        _collectionsProvider.PickupOffers.InsertOne(offer);
    }

    public void ApproveTrade(string tradeOfferID, bool isManager = false)
    {
        UpdateDefinition<TradeOffer> update = isManager ?
            Builders<TradeOffer>.Update.Set(trade => trade.IsManagerApproved, true) :
            Builders<TradeOffer>.Update.Set(trade => trade.IsEmployeeApproved, true);

        var result = _collectionsProvider.TradeOffers.FindOneAndUpdate(offer => offer.Id.ToString() == tradeOfferID, update, new() { ReturnDocument = ReturnDocument.After });

        if (result == null)
        {
            throw new Exception(ErrorUtils.FormatObjectDoesNotExistErrorString(tradeOfferID, _collectionsProvider.TradeOffers.CollectionNamespace.CollectionName));
        }

        if (result.IsManagerApproved == true && result.IsEmployeeApproved == true)
        {
            ExecuteTrade(result);
            // Notify employees
        }
        else if (result.IsManagerApproved == true)
        {
            // Notify employees
        }
        else if (result.IsEmployeeApproved == true)
        {
            // Notify manager
        }

    }
    public void DenyTrade(string tradeOfferID, bool isManager = false)
    {
        var result = _collectionsProvider.TradeOffers.FindOneAndDelete(offer => offer.Id.ToString() == tradeOfferID);
        if (result == null)
        {
            throw new Exception(ErrorUtils.FormatObjectDoesNotExistErrorString(tradeOfferID, _collectionsProvider.TradeOffers.CollectionNamespace.CollectionName));
        }
        if (isManager == true)
        {
            // Notify employees
        }
        else
        {
            // Notify person that offered trade
        }

    }
    private PickupOffer ActOnPickup(string pickupOfferId, bool isApproved)
    {
        var pickup = DBEntityUtils.ThrowIfNotExists(_collectionsProvider.PickupOffers, pickupOfferId);
        if (pickup.IsManagerApproved != null)
        {
            throw new Exception("Pickup has already been acted on. Please submit a new request.");
        }
        var update = Builders<PickupOffer>.Update.Set(trade => trade.IsManagerApproved, isApproved);
        pickup = _collectionsProvider.PickupOffers.FindOneAndUpdate(pickup => pickup.Id.ToString() == pickupOfferId, update);
        if (pickup == null)
        {
            throw new Exception(ErrorUtils.FormatObjectDoesNotExistErrorString(pickupOfferId, _collectionsProvider.PickupOffers.CollectionNamespace.CollectionName));
        }
        return pickup;

    }
    public void ApprovePickup(string pickupOfferId)
    {
        var pickupOffer=ActOnPickup(pickupOfferId, true);
        _scheduler.AssignShift(new ShiftAssignment
        {
            EmployeeID = pickupOffer.EmployeeID,
            ShiftID = pickupOffer.OpenShiftID
        });
    }
    public void DenyPickup(string pickupOfferId)
    {
        ActOnPickup(pickupOfferId, false);
    }
}
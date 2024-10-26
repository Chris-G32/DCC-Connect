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
    public void ApproveTrade(string tradeOfferID, bool isManager = false);
    public void ApprovePickup(string tradeOfferID);
    public void DenyTrade(string tradeOfferID, bool isManager = false);
    public void DenyPickup(string tradeOfferID);

}
public class ShiftTrader : IShiftTrader
{
    private readonly ICollectionsProvider _collectionsProvider;
    private readonly IShiftRetriever _shiftRetriever;
    private readonly IAvailabiltyService _availabilityService;
    private readonly IShiftScheduler _scheduler;
    private readonly IDBTransactionService _transactionService;
    public ShiftTrader(ICollectionsProvider collectionProvider, IDBTransactionService transactionService, IShiftScheduler scheduler, IShiftRetriever shiftRetriever, IAvailabiltyService availabilityService)
    {
        _collectionsProvider = collectionProvider;
        _shiftRetriever = shiftRetriever;
        _availabilityService = availabilityService;
        _scheduler = scheduler;
        _transactionService = transactionService;

        //Add triggers to db
        _ = RegisterUpdateCallbacks();
    }
    private async Task RegisterUpdateCallbacks()
    {
        using var cursor = await _collectionsProvider.TradeOffers.WatchAsync();

        while (await cursor.MoveNextAsync())
        {
            foreach (var change in cursor.Current)
            {
                if (change.OperationType == ChangeStreamOperationType.Update)
                {
                    TradeRequestUpdateCallback(change);
                }
            }
        }
    }
    private void ExecuteTrade(TradeOffer tradeOffer)
    {
        // Unassign the shift from the coverage request 
        var coverageRequest =DBEntityUtils.ThrowIfNotExists(_collectionsProvider.CoverageRequests, tradeOffer.CoverageRequestID);
        var coverageRequestShift = DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, coverageRequest.ShiftID);
        _scheduler.UnassignShift(coverageRequest.ShiftID);

        //Unassign the shift offered for the trade
        var offeredShift=DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, tradeOffer.ShiftOfferedID);
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
    }
    private void TradeRequestUpdateCallback(ChangeStreamDocument<TradeOffer> update)
    {
        var tradeOffer = update.FullDocument;
        if (tradeOffer.IsManagerApproved == true && tradeOffer.IsEmployeeApproved == true)
        {
            _transactionService.RunTransaction(() => ExecuteTrade(tradeOffer));
        }
        if (tradeOffer.IsManagerApproved == false)
        {
            //Notify employees of trade offer denial
        }
        else if (tradeOffer.IsEmployeeApproved == false)
        {
            //Notify employee offering trade that it was denied.
        }
    }
    public void RequestCoverage(CoverageRequest request)
    {
        var shift = DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, request.ShiftID);
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
        var offeredShift = DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, offer.ShiftOfferedID);
        var coverageReq = DBEntityUtils.ThrowIfNotExists(_collectionsProvider.CoverageRequests, offer.CoverageRequestID);
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

    public void PickupShift(PickupOfferWithOptions offer) //Logic breakdown. Validate that the pertinent ID's exist. Then validate shift is actually open.
    {
        DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Employees, offer.EmployeeID);
        var openShift = DBEntityUtils.ThrowIfNotExists(_collectionsProvider.Shifts, offer.OpenShiftID);
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

        var result = _collectionsProvider.TradeOffers.UpdateOne(offer => offer.Id.ToString() == tradeOfferID, update);

        if (result.ModifiedCount == 0)
        {
            throw new Exception(ErrorUtils.FormatObjectDoesNotExistErrorString(tradeOfferID, _collectionsProvider.TradeOffers.CollectionNamespace.CollectionName));
        }
    }

    public void ApprovePickup(string tradeOfferID)
    {
        throw new NotImplementedException();
    }

    public void DenyTrade(string tradeOfferID, bool isManager = false)
    {
        throw new NotImplementedException();
    }

    public void DenyPickup(string tradeOfferID)
    {
        throw new NotImplementedException();
    }
}
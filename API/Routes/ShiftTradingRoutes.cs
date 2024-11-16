using API.Constants;
using API.Models;
using API.Services;
using Carter;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace API.Routes;

public class ShiftTradingRoutes : CarterModule
{
    private readonly IShiftTrader _trader;
    private readonly ILogger<ShiftTradingRoutes> _logger;
    public ShiftTradingRoutes(IShiftTrader trader,ILogger<ShiftTradingRoutes> logger) : base()
    {
        _trader = trader;
        _logger = logger;
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(RouteConstants.OfferUpShiftRoute, RequestCoverage);
        app.MapPut(RouteConstants.TradeShiftRoute, TradeShift);
        app.MapPut(RouteConstants.PickUpShiftRoute, PickupShift);
        app.MapPut(RouteConstants.CancelOfferUpShiftRoute, CancelCoverageRequest);

        // Approval and denial routes
        app.MapPut(RouteConstants.ApproveTradeRoute, ApproveTrade);
        app.MapPut(RouteConstants.DenyTradeRoute, DenyTrade);
        app.MapPut(RouteConstants.ApprovePickupRoute, ApprovePickup);
        app.MapPut(RouteConstants.DenyPickupRoute, DenyPickup);
    }

    public async Task<IResult> ApproveTrade(string tradeOfferId, HttpRequest request)
    {
        try
        {
            _trader.ApproveTrade(tradeOfferId,!string.IsNullOrEmpty(request.Headers.Authorization));
        }
        catch (Exception e) {
            return Results.Problem("Error approving trade.");
        }
        return Results.Ok("Trade approved!");
    }
    public async Task<IResult> DenyTrade(string tradeOfferId, HttpRequest request)
    {
        try
        {
            _trader.DenyTrade(tradeOfferId);
        }
        catch (Exception e)
        {
            return Results.Problem("Error denying trade.");
        }
        return Results.Ok("Trade denied!");
    }
    public async Task<IResult> ApprovePickup(string pickupOfferId, HttpRequest request)
    {
        try
        {
            _trader.ApprovePickup(pickupOfferId);
        }
        catch (Exception e)
        {
            return Results.Problem("Error approving pickup.");
        }
        return Results.Ok("Trade approved!");
    }
    public async Task<IResult> DenyPickup(string pickupOfferId, HttpRequest request)
    {
        try
        {
            _trader.DenyPickup(pickupOfferId);
        }
        catch (Exception e)
        {
            return Results.Problem("Error denying pickup.");
        }
        return Results.Ok("Pickup denied!");
    }
    
    public async Task<IResult> RequestCoverage(CoverageRequest coverage, HttpRequest request)
    {
        try
        {
            _trader.RequestCoverage(coverage);
        }
        catch (Exception e)
        {
            return Results.Problem("Error processing coverage request.");
        }

        return Results.Ok("Coverage requested!");
    }
    public async Task<IResult> TradeShift(TradeOffer offer, HttpRequest request)// TODO
    {
        try
        {
            _trader.OfferTrade(offer);
        }
        catch (Exception e)
        {
            return Results.Problem("Error unassigning shift");
        }

        return Results.Ok("Assignment removed!");
    }
    public async Task<IResult> PickupShift(PickupOffer offer, HttpRequest request)// TODO
    {
        try
        {
            _trader.PickupShift(offer);
        }
        catch (Exception e)
        {
            return Results.Problem("Error unassigning shift");
        }

        return Results.Ok("Assignment removed!");
    }
    public async Task<IResult> CancelCoverageRequest(string offerID, HttpRequest request)
    {
        return Results.Problem("Not yet implemented.");
    }
}

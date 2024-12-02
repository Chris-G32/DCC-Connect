using API.Constants;
using API.Models.Scheduling.Coverage;
using API.Models.Scheduling.Trading;
using API.Services;
using API.Utils;
using Carter;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System.Security.Claims;

namespace API.Routes;

public class ShiftTradingRoutes : CarterModule
{
    private readonly IShiftTrader _trader;
    private readonly ILogger<ShiftTradingRoutes> _logger;
    public ShiftTradingRoutes(IShiftTrader trader, ILogger<ShiftTradingRoutes> logger) : base()
    {
        _trader = trader;
        _logger = logger;
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        RequireAuthorization(PolicyConstants.EmployeePolicy);
        app.MapPut(RouteConstants.OfferUpShiftRoute, RequestCoverage);
        app.MapPut(RouteConstants.TradeShiftRoute, TradeShift);
        app.MapPut(RouteConstants.PickUpShiftRoute, PickupShift);
        app.MapPut(RouteConstants.CancelOfferUpShiftRoute, CancelCoverageRequest);

        // Approval and denial routes
        app.MapPut(RouteConstants.ApproveTradeRoute, ApproveTrade);
        app.MapPut(RouteConstants.DenyTradeRoute, DenyTrade);
        app.MapPut(RouteConstants.ApprovePickupRoute, ApprovePickup)
            .RequireAuthorization(PolicyConstants.ManagerPolicy);
        app.MapPut(RouteConstants.DenyPickupRoute, DenyPickup)
            .RequireAuthorization(PolicyConstants.ManagerPolicy);
    }

    // Secured
    public async Task<IResult> ApproveTrade(string tradeOfferId, HttpRequest request)
    {
        try
        {
            var claims = AuthUtils.GetClaims(request);
            _trader.ApproveTrade(tradeOfferId, claims);
        }
        catch (Exception e)
        {
            return Results.Problem("Error approving trade.");
        }
        return Results.Ok("Trade approved!");
    }
    //Secured
    public async Task<IResult> DenyTrade(string tradeOfferId, HttpRequest request)
    {
        try
        {
            var claims = AuthUtils.GetClaims(request);
            _trader.DenyTrade(tradeOfferId, claims);
        }
        catch (Exception e)
        {
            return Results.Problem("Error denying trade.");
        }
        return Results.Ok("Trade denied!");
    }
    //Secured
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
    //secured
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

    //Secured
    public async Task<IResult> RequestCoverage(CoverageRequestInfo info, HttpRequest request)
    {
        try
        {
            var claims = AuthUtils.GetClaims(request);
            _trader.RequestCoverage(info, claims);
        }
        catch (Exception e)
        {
            return Results.Problem("Error processing coverage request.");
        }

        return Results.Ok("Coverage requested!");
    }
    public async Task<IResult> TradeShift(TradeOfferCreationInfo offer, HttpRequest request)// TODO
    {
        try
        {
            var claims = AuthUtils.GetClaims(request);
            _trader.OfferTrade(offer, claims);
        }
        catch (Exception e)
        {
            return Results.Problem("Error unassigning shift");
        }

        return Results.Ok("Assignment removed!");
    }
    public async Task<IResult> PickupShift(PickupOfferCreationInfo offer, HttpRequest request)// TODO
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

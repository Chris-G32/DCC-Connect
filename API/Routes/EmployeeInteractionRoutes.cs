using API.Constants;
using API.Models;
using API.Services;
using Carter;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace API.Routes;

public class EmployeeInteractionRoutes : CarterModule
{
    private readonly IShiftTrader _trader;
    public EmployeeInteractionRoutes(IShiftTrader trader) : base()
    {
        _trader = trader;
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(RouteConstants.OfferUpShiftRoute, RequestCoverage);
        app.MapPut(RouteConstants.TradeShiftRoute, TradeShift);
        app.MapPut(RouteConstants.PickUpShiftRoute, PickupShift);
        app.MapPut(RouteConstants.CancelOfferUpShiftRoute, CancelCoverageRequest);
    }
    public async Task<IResult> RequestCoverage(CoverageRequest coverage, HttpRequest request)// TODO
    {
        try
        {
            _trader.RequestCoverage(coverage);
        }
        catch (Exception e)
        {
            return Results.Problem("Error unassigning shift");
        }

        return Results.Ok("Assignment removed!");
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
    public async Task<IResult> PickupShift(PickupOfferWithOptions offer, HttpRequest request)// TODO
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

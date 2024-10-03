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
        app.MapPut(RouteConstants.OfferUpShiftRoute, OfferUpShift);
        app.MapPut(RouteConstants.CancelOfferUpShiftRoute, CancelOfferUpShift);
    }
    public async Task<IResult> OfferUpShift(ShiftOffer offer, HttpRequest request)
    {
        try
        {
            _trader.OfferUpShift(offer);
        }
        catch (Exception e)
        {
            return Results.Problem("Error unassigning shift");
        }

        return Results.Ok("Assignment removed!");
    }
    public async Task<IResult> CancelShiftOffer(string offerID, HttpRequest request)
    {

    }
}

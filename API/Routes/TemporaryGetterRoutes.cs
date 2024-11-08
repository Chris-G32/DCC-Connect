using API.Constants;
using API.Services;
using Carter;
using API.Models;
using MongoDB.Driver;
using MongoDB.Bson;
namespace API.Routes;

/// <summary>
/// These are quick shitty endpoints to be used for testing only. 
/// No intent that the actual implementation of these share any commonality.
/// </summary>
public class TemporaryGetterRoutes : CarterModule
{
    private readonly ICollectionsProvider _collectionsProvider;
    public TemporaryGetterRoutes(ICollectionsProvider cp) : base()
    {
        _collectionsProvider = cp;
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("employees/all", (HttpResponse res) =>
        {
            var filter = Builders<Employee>.Filter.Empty;
            var temp = _collectionsProvider.Employees.Find(filter).ToList();
            
            return temp;
        });
        app.MapGet("shifts/all", (HttpResponse res) =>
        {
            var filter = Builders<Shift>.Filter.Empty;
            var temp = _collectionsProvider.Shifts.Find(filter).ToList();
            return temp;
        });
        app.MapGet("coveragerequest/all", (HttpResponse res) =>
        {
            var filter = Builders<CoverageRequest>.Filter.Empty;
            return _collectionsProvider.CoverageRequests.Find(filter).ToList();
        });
        app.MapGet("tradeoffers/all", (HttpResponse res) =>
        {
            var filter = Builders<TradeOffer>.Filter.Empty;
            return _collectionsProvider.TradeOffers.Find(filter).ToList();
        });
        app.MapGet("pickupoffers/all", (HttpResponse res) =>
        {
            var filter = Builders<PickupOffer>.Filter.Empty;
            return _collectionsProvider.PickupOffers.Find(filter).ToList();
        });
        app.MapGet("timeoffrequests/all", (HttpResponse res) =>
        {
            var filter = Builders<TimeOffRequest>.Filter.Empty;
            return _collectionsProvider.TimeOffRequests.Find(filter).ToList();
        });
    }

}
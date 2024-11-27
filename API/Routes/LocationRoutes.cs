using API.Constants;
using API.Models.ShiftLocations;
using API.Services;
using Carter;
using MongoDB.Driver;

namespace API.Routes;

public class LocationRoutes(ILogger<LocationRoutes> logger, ICollectionsProvider cp) : CarterModule
{
    private readonly ICollectionsProvider _collectionsProvider = cp;
    private readonly ILogger<LocationRoutes> _logger = logger;
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(RouteConstants.GetLocationsRoute, (HttpRequest request) =>
        {
            return _collectionsProvider.ShiftLocations.Find(Builders<ShiftLocation>.Filter.Empty).ToList();
        });
    }
}

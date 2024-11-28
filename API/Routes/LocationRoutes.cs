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
        // This is more or less an endpoint that exists to add stateful constants for the locations of shifts. Low change rate.
        app.MapGet(RouteConstants.GetLocationsRoute, (HttpRequest request) =>
        {
            return _collectionsProvider.ShiftLocations.Find(Builders<ShiftLocation>.Filter.Empty).ToList();
        }).RequireAuthorization("EmployeePolicy");
    }
}

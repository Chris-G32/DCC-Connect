
using API.Constants;
using API.Models;
using API.Services;
using API.Utils;
using Carter;
using MongoDB.Driver;

namespace API.Routes;

public class TimeOffRoutes(ILogger<TimeOffRoutes> logger, ICollectionsProvider cp) : CarterModule
{
    private readonly ICollectionsProvider _collectionsProvider = cp;
    private readonly ILogger<TimeOffRoutes> _logger = logger;
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        RequireAuthorization(PolicyConstants.EmployeePolicy);
        app.MapPost(TimeOffRouteConstants.RequestTimeOffRoute, (TimeRange timeOff, HttpRequest request) =>
        {
            try
            {
                var claims = AuthUtils.GetClaims(request);
                _collectionsProvider.TimeOffRequests.InsertOne(new TimeOffRequest() { TimeOffTimeSpan = timeOff, EmployeeID = claims.UserID });
                return Results.Ok("Successfully requested off.");
            }
            catch (Exception e)
            {
                return Results.Problem("Failed to request off.");
            }
        });
        app.MapGet(TimeOffRouteConstants.GetTimeOffRequestsRoute,(HttpRequest request) =>
        {
            var claims = AuthUtils.GetClaims(request);
            var requests=_collectionsProvider.TimeOffRequests.Find(x => x.EmployeeID == claims.UserID).ToList();
            return Results.Ok(requests);
        });
    }
}

using API.Constants;
using API.Errors;
using API.Models.QueryOptions;
using API.Models;
using API.Services.QueryExecuters;
using Carter;
using MongoDB.Bson;
using MongoDB.Driver;
using API.Models.Shifts;
using API.Services;
using API.Utils;

namespace API.Routes.ShiftRoutes;

public class ShiftRoutes(ILogger<ShiftRoutes> logger, IShiftQueryExecuter shiftRetriever, IShiftScheduler scheduler) : CarterModule
{
    private readonly IShiftQueryExecuter _shiftRetriever = shiftRetriever;
    private readonly ILogger<ShiftRoutes> _logger = logger;
    private readonly IShiftScheduler _shiftScheduler = scheduler;

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(RouteConstants.GetShiftRoute, GetShifts);
        app.MapGet(RouteConstants.GetShiftByIdRoute, GetShiftById);
        app.MapPost(RouteConstants.CreateShiftRoute, CreateShift);
        app.MapDelete(RouteConstants.DeleteShiftRoute, DeleteShift);
    }
    /// <summary>
    /// Creates a new shift in the database.
    /// </summary>
    /// <param name="shift"> The shifts information. Any id parameter provided here will be ignored and set to null.</param>
    /// <param name="request"></param>
    /// <returns> Whether the shift was successfully created or not.</returns>
    public async Task<IResult> CreateShift(ShiftCreationInfo shift, HttpRequest request)
    {
        try
        {
            Shift createdShift;
            _shiftScheduler.CreateShift(shift, out createdShift);
            return Results.Created(RouteConstants.GetShiftRoute + createdShift.Id.ToString(), createdShift);
        }
        catch (DCCApiException e)
        {
            _logger.LogWarning($"Request to \"{RouteConstants.CreateShiftRoute}\" failed gracefully.");
            return Results.Problem($"Failed to create shift: {e.Message}");
        }
        catch (MongoException e)
        {
            _logger.LogError(e, "Error with Mongo DB occurred");
            return Results.Problem(ErrorConstants.ErrorInMongoDB);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in \"{RouteConstants.GetShiftRoute}\": {e.Message}");
            return Results.Problem();
        }
    }

    /// <summary>
    /// Deletes a shift from the database. Must be unassigned first.
    /// </summary>
    /// <param name="shiftID">Shift id to delete</param>
    /// <param name="request"></param>
    /// <returns>Whether the operation succeeded or not.</returns>
    public async Task<IResult> DeleteShift(string shiftID, HttpRequest request)
    {
        try
        {
            var shiftObjectId = ObjectId.Parse(shiftID);
            _shiftScheduler.DeleteShift(shiftObjectId);
            return Results.NoContent();
        }
        catch (DCCApiException e)
        {
            _logger.LogWarning($"Request to \"{RouteConstants.DeleteShiftRoute}\" failed gracefully.");
            return Results.Problem($"Failed to delete shift: {e.Message}");
        }
        catch (MongoException e)
        {
            _logger.LogError(e, "Error with Mongo DB occurred");
            return Results.Problem(ErrorConstants.ErrorInMongoDB);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in \"{RouteConstants.GetShiftRoute}\": {e.Message}");
            return Results.Problem();
        }
    }

    /// <summary>
    /// Retrieves shifts based on query filters.
    /// </summary>
    /// <param name="filter">The query options for filtering shifts.</param>
    /// <param name="request">The HTTP request containing additional details.</param>
    /// <returns>A list of shifts matching the query; otherwise, an error result.</returns>
    public async Task<IResult> GetShifts(DateTime? startAvailability, DateTime? endAvailability, string? assignedEmployeeId, string? requiredRole, bool? openShiftsOnly, HttpRequest request)
    {
        try
        {

            TimeRange? range = null;
            //If both start and end are provided, assing to range.
            if (startAvailability != null && endAvailability != null)
            {
                range = new TimeRange(startAvailability.Value, endAvailability.Value);
            }
            //This is true if only one of the two is provided.
            else if (startAvailability != endAvailability)
            {
                return Results.BadRequest("startAvailability and endAvailability must be provided together.");
            }
            var claims = AuthUtils.GetClaims(request);
            if (claims.Role == RoleConstants.Employee)
            {
                assignedEmployeeId = claims.UserID.ToString();
            }
            var options = new ShiftQueryOptions { TimeFilter = range, EmployeeIDFilterString = assignedEmployeeId, RequiredRoleFilter = requiredRole };
            var result = openShiftsOnly == true ? _shiftRetriever.GetOpenShifts(options) : _shiftRetriever.GetShifts(options);
            _logger.LogInformation($"Successfully processed request to \"{RouteConstants.GetShiftRoute}\"");
            return Results.Ok(result);
        }
        catch (DCCApiException e)
        {
            _logger.LogWarning($"Request to \"{RouteConstants.GetShiftRoute}\" failed gracefully.");
            return Results.Problem(e.Message);
        }
        catch (MongoException e)
        {
            _logger.LogError(e, "Error with Mongo DB occurred");
            return Results.Problem(ErrorConstants.ErrorInMongoDB);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in \"{RouteConstants.GetShiftRoute}\": {e.Message}");
            return Results.Problem();
        }
    }
    /// <summary>
    /// Retrieves a shift by its ID.
    /// </summary>
    /// <param name="id">The ID of the shift to retrieve.</param>
    /// <param name="request">The HTTP request containing additional details.</param>
    /// <returns>The shift details if found; otherwise, an error result.</returns>
    public async Task<IResult> GetShiftById(string id, HttpRequest request)
    {
        try
        {
            return Results.Ok(_shiftRetriever.GetShift(ObjectId.Parse(id)));
        }
        catch (DCCApiException e)
        {
            return Results.Problem(e.Message);
        }
        catch (MongoException e)
        {
            _logger.LogError(e, "Error with Mongo DB occurred");
            return Results.Problem(ErrorConstants.ErrorInMongoDB);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in {RouteConstants.GetShiftByIdRoute}: {e.Message}");
            return Results.Problem();
        }
    }
}

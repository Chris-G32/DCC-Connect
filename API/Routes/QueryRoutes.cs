using API.Constants;
using API.Errors;
using API.Models;
using API.Models.QueryOptions;
using API.Services;
using API.Services.QueryExecuters;
using Carter;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Routes;

/// <summary>
/// Contains routes for retrieving and querying data from the database.
/// </summary>
/// <param name="logger">The logger instance for logging information and errors.</param>
/// <param name="shiftRetriever">Service for executing shift-related database queries.</param>
/// <param name="employeeRetriever">Service for executing employee-related database queries.</param>
public class QueryRoutes(ILogger<QueryRoutes> logger, IShiftQueryExecuter shiftRetriever, IEmployeeQueryExecuter employeeRetriever, ICoverageRequestQueryExecuter covReqQueryer) : CarterModule
{
    private readonly IShiftQueryExecuter _shiftRetriever = shiftRetriever;
    private readonly IEmployeeQueryExecuter _employeeRetriever = employeeRetriever;
    private readonly ICoverageRequestQueryExecuter _coverageRequestRetriever = covReqQueryer;
    private readonly ILogger<QueryRoutes> _logger = logger;

    /// <summary>
    /// Provides a reusable result for MongoDB-related errors.
    /// </summary>
    public readonly Func<IResult> MongoProblem = () => Results.Problem(ErrorConstants.ErrorInMongoDB);

    /// <summary>
    /// Adds route mappings for querying and retrieving data.
    /// </summary>
    /// <param name="app">The endpoint route builder used to map routes.</param>
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(RouteConstants.GetShiftByIdRoute, GetShiftById);
        app.MapGet(RouteConstants.GetEmployeeByIdRoute, GetEmployeeById);
        app.MapPost(RouteConstants.GetEmployeeRoute, GetEmployees);
        app.MapPost(RouteConstants.GetShiftRoute, GetShifts);
        app.MapPost(RouteConstants.GetOpenShiftRoute, GetOpenShifts);
        app.MapPost(RouteConstants.GetCoverageRequestRoute, GetCoverageRequests);
        //app.MapPost(RouteConstants.GetTradesRoute, );
        //app.MapPost(RouteConstants.GetPickupsRoute, );
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
            return MongoProblem();
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in {RouteConstants.GetShiftByIdRoute}: {e.Message}");
            return Results.Problem();
        }
    }
    /// <summary>
    /// Retrieves an employee by its ID.
    /// </summary>
    /// <param name="id">The ID of the employee to retrieve.</param>
    /// <param name="request">The HTTP request containing additional details.</param>
    /// <returns>The employees info if found; otherwise, an error result.</returns>
    public async Task<IResult> GetEmployeeById(string id, HttpRequest request)
    {
        try
        {
            return Results.Ok(_employeeRetriever.GetEmployee(ObjectId.Parse(id)));
        }
        catch (DCCApiException e)
        {
            return Results.Problem(e.Message);
        }
        catch (MongoException e)
        {
            _logger.LogError(e, "Error with Mongo DB occurred");
            return MongoProblem();
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in {RouteConstants.GetEmployeeByIdRoute}: {e.Message}");
            return Results.Problem();
        }
    }

    /// <summary>
    /// Retrieves employees based on query filters.
    /// </summary>
    /// <param name="filter">The query options for filtering employees.</param>
    /// <param name="request">The HTTP request containing additional details.</param>
    /// <returns>A list of employees matching the query; otherwise, an error result.</returns>
    public async Task<IResult> GetEmployees(EmployeeQueryOptions filter, HttpRequest request)
    {
        try
        {
            var result = _employeeRetriever.GetEmployees(filter);
            _logger.LogInformation($"Successfully processed request to \"{RouteConstants.GetEmployeeRoute}\"");
            return Results.Ok(result);
        }
        catch (DCCApiException e)
        {
            _logger.LogWarning($"Request to \"{RouteConstants.GetEmployeeRoute}\" failed gracefully.");
            return Results.Problem(e.Message);
        }
        catch (MongoException e)
        {
            _logger.LogError(e, "Error with Mongo DB occurred");
            return MongoProblem();
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in \"{RouteConstants.GetEmployeeRoute}\": {e.Message}");
            return Results.Problem();
        }
    }

    /// <summary>
    /// Retrieves shifts based on query filters.
    /// </summary>
    /// <param name="filter">The query options for filtering shifts.</param>
    /// <param name="request">The HTTP request containing additional details.</param>
    /// <returns>A list of shifts matching the query; otherwise, an error result.</returns>
    public async Task<IResult> GetShifts(ShiftQueryOptions filter, HttpRequest request)
    {
        try
        {
            var result = _shiftRetriever.GetShifts(filter);
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
            return MongoProblem();
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in \"{RouteConstants.GetShiftRoute}\": {e.Message}");
            return Results.Problem();
        }
    }

    /// <summary>
    /// Retrieves open shifts based on query filters.
    /// </summary>
    /// <param name="filter">The query options for filtering open shifts.</param>
    /// <param name="request">The HTTP request containing additional details.</param>
    /// <returns>A list of open shifts matching the query; otherwise, an error result.</returns>
    public async Task<IResult> GetOpenShifts(ShiftQueryOptions filter, HttpRequest request)
    {
        try
        {
            var result = _shiftRetriever.GetOpenShifts(filter);
            _logger.LogInformation($"Successfully processed request to \"{RouteConstants.GetOpenShiftRoute}\"");
            return Results.Ok(result);
        }
        catch (DCCApiException e)
        {
            _logger.LogWarning($"Request to \"{RouteConstants.GetOpenShiftRoute}\" failed gracefully.");
            return Results.Problem(e.Message);
        }
        catch (MongoException e)
        {
            _logger.LogError(e, "Error with Mongo DB occurred");
            return MongoProblem();
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in \"{RouteConstants.GetOpenShiftRoute}\": {e.Message}");
            return Results.Problem();
        }
    }
    /// <summary>
    /// Retrieves employees based on query filters.
    /// </summary>
    /// <param name="filter">The query options for filtering employees.</param>
    /// <param name="request">The HTTP request containing additional details.</param>
    /// <returns>A list of employees matching the query; otherwise, an error result.</returns>
    public async Task<IResult> GetCoverageRequests(CoverageRequestQueryOptions filter, HttpRequest request)
    {
        try
        {
            var result = _coverageRequestRetriever.GetCoverageRequests(filter);
            _logger.LogInformation($"Successfully processed request to \"{RouteConstants.GetCoverageRequestRoute}\"");
            return Results.Ok(result);
        }
        catch (DCCApiException e)
        {
            _logger.LogWarning($"Request to \"{RouteConstants.GetCoverageRequestRoute}\" failed gracefully.");
            return Results.Problem(e.Message);
        }
        catch (MongoException e)
        {
            _logger.LogError(e, "Error with Mongo DB occurred");
            return MongoProblem();
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Unexpected exception thrown in \"{RouteConstants.GetCoverageRequestRoute}\": {e.Message}");
            return Results.Problem();
        }
    }
}

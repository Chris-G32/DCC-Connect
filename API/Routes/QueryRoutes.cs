using API.Constants;
using API.Errors;
using API.Models;
using API.Models.QueryOptions;
using API.Services;
using API.Services.QueryExecuters;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel;

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
        // GET routes
        app.MapGet(RouteConstants.GetEmployeeByIdRoute, GetEmployeeById);
        app.MapGet(RouteConstants.GetCoverageRequestByIdRoute, GetCoverageRequestById);
        // POST routes
        app.MapGet(RouteConstants.GetEmployeeRoute, GetEmployees);
        app.MapGet(RouteConstants.GetCoverageRequestRoute, GetCoverageRequests);
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
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IResult> GetCoverageRequestById(string id, HttpRequest request)
    {
        try
        {
            return Results.Ok(_coverageRequestRetriever.GetCoverageRequest(ObjectId.Parse(id)));
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
            _logger.LogCritical($"Unexpected exception thrown in {RouteConstants.GetCoverageRequestByIdRoute}: {e.Message}");
            return Results.Problem();
        }
    }

    /// <summary>
    /// Retrieves employees based on query filters.
    /// </summary>
    /// <param name="startAvailability">Beginning of a period to find employees that are available during</param>
    /// <param name="endAvailability">End of avaliablity period</param>
    /// <param name="employeeRole">The role of employees to filter by</param>
    /// <param name="request"></param>
    /// <returns>A list of employees matching the query; otherwise, an error result</returns>
    public async Task<IResult> GetEmployees(DateTime? startAvailability, DateTime? endAvailability, string? employeeRole, HttpRequest request)
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

            var employeeQueryOptions = new EmployeeQueryOptions { EmployeeRole = employeeRole, TimeFilter = range };
            var result = _employeeRetriever.GetEmployees(employeeQueryOptions);
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
    public async Task<IResult> GetCoverageRequests(DateTime? startAvailability, DateTime? endAvailability, CoverageOptions? coverageType, string? requiredRole, string? employeeID, HttpRequest request)
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

            var filter = new CoverageRequestQueryOptions { TimeFilter = range, CoverageType = coverageType, RequiredRoleFilter = requiredRole, EmployeeIDFilterString = employeeID };
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

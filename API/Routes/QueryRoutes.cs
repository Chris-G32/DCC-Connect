using API.Constants;
using API.Errors;
using API.Models;
using API.Models.QueryOptions;
using API.Services.QueryExecuters;
using Carter;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace API.Routes;

public class QueryRoutes(ILogger<QueryRoutes> logger,IShiftQueryExecuter shiftRetriever,IEmployeeQueryExecuter employeeRetriever) : CarterModule
{
    private readonly IShiftQueryExecuter _shiftRetriever= shiftRetriever;
    private readonly IEmployeeQueryExecuter  _employeeRetriever= employeeRetriever;
    private readonly ILogger<QueryRoutes> _logger=logger;

    /// <summary>
    /// Gets an object
    /// </summary>
    /// <typeparam name="FilterType">Can not be an interface, swagger will need to serialize this type from the endpoints.</typeparam>
    /// <typeparam name="ReturnType">Return type of the getter function</typeparam>
    /// <param name="app">app to add route to</param>
    /// <param name="route">route for the gettter endpoint </param>
    /// <param name="getter">Gets the object(s) from db</param>
    private void AddGetterRoute<FilterType,ReturnType>(IEndpointRouteBuilder app,string route,Func<FilterType,ReturnType> getter)
    {
        app.MapPost(route, (FilterType filter,HttpRequest request) =>
        {
            try
            {
                var result = Results.Ok(getter(filter));
                _logger.LogInformation($"Successfully processed request to \"{route}\"");
                return result ;
            }
            catch (DCCApiException e)
            {
                _logger.LogWarning($"Request to \"{route}\" failed gracefully.");
                return Results.Problem(e.Message);
            }
            catch (MongoException e)
            {
                _logger.LogError(e,"Error with Mongo DB Occurred");
                return Results.Problem("Error in database, retry request.");
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Unexpected exception thrown in {route}: {e.Message}");
                return Results.Problem();
            }
        });
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        AddGetterRoute<EmployeeQueryOptions,List<Employee>>(app, RouteConstants.GetEmployeeRoute, _employeeRetriever.GetEmployees);
        AddGetterRoute<ShiftQueryOptions, List<Shift>>(app, RouteConstants.GetShiftRoute, _shiftRetriever.GetShifts);
        AddGetterRoute<ShiftQueryOptions, List<Shift>>(app, RouteConstants.GetOpenShiftRoute, _shiftRetriever.GetOpenShifts);

    }
}

using API.Constants;
using API.Errors;
using API.Models.QueryOptions;
using API.Models.Shifts;
using API.Services;
using API.Services.QueryExecuters;
using Carter;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace API.Routes;

public class ShiftSchedulingRoutes : CarterModule
{
    IDatabaseProvider _databaseProvider;
    private readonly IShiftScheduler _shiftScheduler;
    private readonly IShiftQueryExecuter _retriever;
    private readonly ILogger<ShiftSchedulingRoutes> _logger;
    public ShiftSchedulingRoutes(IDatabaseProvider dbProvider, IShiftScheduler scheduler, IShiftQueryExecuter retriever, ILogger<ShiftSchedulingRoutes> logger) : base()
    {
        _databaseProvider = dbProvider;
        _shiftScheduler = scheduler;
        _retriever = retriever;
        _logger = logger;
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(RouteConstants.AssignShiftRoute, AssignShift);
        app.MapPut(RouteConstants.UnassignShiftRoute, UnassignShift);
    }
    /// <summary>
    /// Unassigns the assigned employee from a shift.
    /// </summary>
    /// <param name="assignmentID"> Shift to remove assignment from.</param>
    /// <param name="request"></param>
    /// <returns> Whether the operation succeeded or not.</returns>
    public async Task<IResult> UnassignShift(string shiftId, HttpRequest request)
    {
        try
        {
            var shiftObjectId=ObjectId.Parse(shiftId);
            _shiftScheduler.UnassignShift(shiftObjectId);
        }
        catch (Exception e)
        {
            return Results.Problem("Error unassigning shift");
        }

        return Results.Ok("Assignment removed!");
    }
    /// <summary>
    /// Assigns an employee to a shift.
    /// </summary>
    /// <param name="assignment"> Contains what shift should be assigned and who to assign </param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IResult> AssignShift(ShiftAssignment assignment, HttpRequest request)
    {
        try
        {
            _shiftScheduler.AssignShift(assignment);
        }
        catch (Exception e)
        {
            return Results.Problem("Failed to assign shift. Maybe someone is already assigned to it?");
        }

        return Results.Ok("Shift assigned!");
    }
}

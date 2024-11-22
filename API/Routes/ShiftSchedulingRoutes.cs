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
        app.MapPut(RouteConstants.CreateShiftRoute, CreateShift);
        app.MapDelete(RouteConstants.DeleteShiftRoute, DeleteShift);

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
        }
        catch (Exception e)
        {
            return Results.Problem($"Error deleting shift: {e.Message}");
        }

        return Results.Ok("Shift deleted!");
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
    /// <summary>
    /// Creates a shift in the database.
    /// </summary>
    /// <param name="shift"> The shifts information. Any id parameter provided here will be ignored and set to null.</param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<IResult> CreateShift(ShiftCreationInfo shift, HttpRequest request)
    {
            
        try
        {
            _shiftScheduler.CreateShift(shift);
        }
        catch (Exception e)
        {
            return Results.Problem("Failed to create shift.");
        }

        return Results.Ok("Shift Successfully Created");
    }
}

using API.Constants;
using API.Errors;
using API.Models;
using API.Models.QueryOptions;
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
        app.MapPut(RouteConstants.DeleteShiftRoute, DeleteShift);

        app.MapPut(RouteConstants.AssignShiftRoute, AssignShift);
        app.MapPut(RouteConstants.UnassignShiftRoute, UnassignShift);

    }
    
    public async Task<IResult> UnassignShift(string assignmentID, HttpRequest request)
    {
        try
        {
            var assignmentObjId=ObjectId.Parse(assignmentID);
            _shiftScheduler.UnassignShift(assignmentObjId);
        }
        catch (Exception e)
        {
            return Results.Problem("Error unassigning shift");
        }

        return Results.Ok("Assignment removed!");
    }
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
    public async Task<IResult> CreateShift(ShiftExternal shift, HttpRequest request)
    {
        try
        {
            _shiftScheduler.CreateShift(new Shift(shift));
        }
        catch (Exception e)
        {
            return Results.Problem("Failed to create shift.");
        }

        return Results.Ok("Shift Successfully Created");
    }
}

using API.Models;
using API.Services;
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
    public ShiftSchedulingRoutes(IDatabaseProvider dbProvider, IShiftScheduler scheduler) : base()
    {
        _databaseProvider = dbProvider;
        _shiftScheduler = scheduler;
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("shift/create", CreateShift);
        app.MapPut("shift/delete", DeleteShift);

        app.MapPut("shift/assign", AssignShift);
        app.MapPut("shift/unassign", UnassignShift);
    }
    public async Task<IResult> UnassignShift(string assignmentID, HttpRequest request)
    {
        try
        {
            _shiftScheduler.unassignShift(assignmentID);
        }
        catch (Exception e)
        {
            return Results.Problem("Error unassinging shift");
        }

        return Results.Ok("Assignment removed!");
    }
    public async Task<IResult> DeleteShift(string shiftID, HttpRequest request)
    {
        try
        {
            _shiftScheduler.deleteShift(shiftID);
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
            _shiftScheduler.assignShift(assignment);
        }
        catch (Exception e)
        {
            return Results.Problem("Failed to assign shift. Maybe someone is already assigned to it?");
        }

        return Results.Ok("Shift assigned!");
    }
    public async Task<IResult> CreateShift(Shift shift, HttpRequest request)
    {
        try
        {
            _shiftScheduler.createShift(shift);
        }
        catch (Exception e)
        {
            return Results.Problem("Failed to create shift.");
        }

        return Results.Ok("Shift Successfully Created");
    }
}

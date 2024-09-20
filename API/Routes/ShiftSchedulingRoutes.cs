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
    public ShiftSchedulingRoutes(IDatabaseProvider dbProvider) : base() { _databaseProvider = dbProvider; }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("shift/create", CreateShift);
        app.MapPut("shift/assign", AssignShift);
        app.MapPut("createEmployee", createEmployee);
    }
    public async Task<IResult> createEmployee(Employee employee)
    {
        var collection = _databaseProvider.Database.GetCollection<Employee>("employees");
        try
        {
            collection.InsertOne(employee);
            return Results.Ok("Created employee successfully.");
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

            
    }
    public async Task<IResult> AssignShift(ShiftAssignment assignment, HttpRequest request)
    {

        // Create or get the employees collection
        var collection = _databaseProvider.Database.GetCollection<Employee>("employees");

        Employee employee = new()
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "555-555-5555",
            Email = "TENP@gmail.com"
        };
        collection.InsertOne(employee);

        return Results.Ok("Shift assigned!");
    }
    public async Task<IResult> CreateShift(Shift shift, HttpRequest request)
    {
        var collection = _databaseProvider.Database.GetCollection<Employee>("employees");
        var res = await collection.FindAsync(o => o.FirstName == "John");

        return Results.Ok(res.First().LastName);
    }
}

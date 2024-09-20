using API.Models;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace API.Routes;

public class ShiftSchedulingRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("test", TestRoute);
    }

    public IResult TestRoute()
    {
        ExampleClass eg = new();
        return Results.Ok(eg);
    }
}

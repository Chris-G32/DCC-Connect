using Carter;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;
using API.Constants;

namespace API.Routes
{
    public class SessionRoutes : CarterModule
    {
        private readonly IAuthService _authService;
        private readonly ILogger<SessionRoutes> _logger;

        public SessionRoutes(IAuthService authService, ILogger<SessionRoutes> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("TestSession", (HttpRequest req) =>
            {
                _logger.LogInformation(req.Headers.Authorization);
                return Results.Ok("Session logged.");
            });

            app.MapPost(RouteConstants.ValidateTokenRoute, ValidateSession);
        }

        // Route for generating a session token with 2FA code
        public async Task<IResult> GenerateSession([FromBody] string userEmail, [FromBody] string code)
        {
            try
            {
                var token = _authService.AuthenticateAndGenerateSession(userEmail, code);
                return Results.Ok(token); // Return the session token
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message); // Handle invalid email or code
            }
        }

        // Route for validating a session token
        public async Task<IResult> ValidateSession([FromBody] string token)
        {
            try
            {
                var isValid = _authService.ValidateSession(token);

                if (isValid)
                {
                    return Results.Ok("Session is valid");
                }
                return Results.Problem("Session is invalid or expired");
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                _logger.LogError($"Error validating session: {ex.Message}");

                // Return a more specific 500 error message
                return Results.Problem("An unexpected error occurred while validating the session.");
            }
        }

    }
}

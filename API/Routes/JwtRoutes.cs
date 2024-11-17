using Carter;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;
using API.Constants;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Routes
{
    public class JwtRoutes : CarterModule
    {
        private readonly IAuthService _authService;

        public JwtRoutes(IAuthService authService)
        {
            _authService = authService;
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost(RouteConstants.GenerateTokenRoute, GenerateToken);
            app.MapPost(RouteConstants.ValidateTokenRoute, ValidateToken);
        }

        // Route for generating JWT token with 2FA code
        public async Task<IResult> GenerateToken(string userEmail,string code)
        {
            // Validate the 2FA code and generate a token
            var token = _authService.AuthenticateAndGenerateToken(userEmail, code);
            return token != null ? Results.Ok(token) : Results.Problem("Invalid 2FA code.");
        }

        // Route for validating JWT token
        public async Task<IResult> ValidateToken([FromBody] string token, [FromQuery] string secret)
        {
            // Validate the provided token using the secret
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return Results.Ok("Token is valid");
            }
            catch
            {
                return Results.Problem("Token is invalid");
            }
        }
    }
}

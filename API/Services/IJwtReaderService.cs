using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace API.Services
{
    // Interface for JWT reading service, providing methods for token validation and employee ID extraction
    public interface IJwtReaderService
    {
        // Validates the JWT token and extracts claims if valid
        ClaimsPrincipal? ValidateToken(string token);

        // Extracts employeeID from a valid JWT token
        ObjectId? GetEmployeeIdFromToken(string token);
    }

    public class IJwtReaderService : IJwtReaderService
    {
        private readonly string _jwtSecret; // Secret key used to validate JWT tokens

        public IJwtReaderService(string jwtSecret)
        {
            _jwtSecret = jwtSecret;
        }

        // Validates the JWT token and extracts claims if valid
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // Ensure the token is signed with the correct key
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // Skip issuer validation
                ValidateAudience = false, // Skip audience validation
                ClockSkew = TimeSpan.Zero // No clock skew
            };

            try
            {
                // Validate the token and extract claims
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal; // Return the token's claims
            }
            catch
            {
                return null; // Return null if validation fails
            }
        }

        // Extracts employeeID from a valid JWT token
        public ObjectId? GetEmployeeIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null; // Return null if token is invalid

            // Extract employeeID claim from token
            var employeeIdClaim = principal.FindFirst("EmployeeID")?.Value;

            return employeeIdClaim != null && ObjectId.TryParse(employeeIdClaim, out var employeeId)
                ? employeeId
                : (ObjectId?)null; // Return the employeeID as an ObjectId or null if invalid
        }
    }
}

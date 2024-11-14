using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace API.Services
{
    public class JwtReaderService
    {
        private readonly string _jwtSecret; // Secret key used to validate JWT tokens

        public JwtReaderService(string jwtSecret)
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
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _); // Validate the token
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

            // Extract employeeID claim
            var employeeIdClaim = principal.FindFirst("EmployeeID")?.Value;

            return employeeIdClaim != null && ObjectId.TryParse(employeeIdClaim, out var employeeId)
                ? employeeId
                : (ObjectId?)null; // Return the employeeID as an ObjectId or null if invalid
        }
    }
}

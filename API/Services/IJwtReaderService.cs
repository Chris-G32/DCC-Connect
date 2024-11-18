using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services
{
    // Interface for JWT reading service, providing methods for token validation and employee ID extraction
    public interface IJwtReaderService
    {
        // Validates the JWT token and extracts claims if valid
        ClaimsPrincipal? ValidateToken(string token, string userEmail);

        // Extracts employeeID from a valid JWT token
        ObjectId? GetEmployeeIdFromToken(string token, string userEmail);
    }

    public class JwtReaderService : IJwtReaderService
    {
        private readonly ICollectionsProvider _collectionsProvider; // For accessing MongoDB collections

        // Constructor that injects the MongoDB collection for users
        public JwtReaderService(ICollectionsProvider cp)
        {
            _collectionsProvider = cp; // Assuming collection is named "Users"
        }

        // Validates the JWT token and extracts claims if valid
        public ClaimsPrincipal? ValidateToken(string token, string userEmail)
        {
            // Fetch the user from the database based on the email
            var user = _collectionsProvider.Users.Find(u => u.Email == userEmail).FirstOrDefault();

            if (user == null)
            {
                // Return null if no user is found with the provided email
                return null;
            }

            // Extract JWT secret from the user object
            var jwtSecret = user.JWTSecret;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

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

                // Optionally, log or verify the email claim matches
                var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;

                if (emailClaim != null && emailClaim != userEmail)
                {
                    // Return null if the email in the token does not match the provided userEmail
                    return null;
                }

                return principal; // Return the token's claims if valid
            }
            catch
            {
                return null; // Return null if validation fails
            }
        }

        // Extracts employeeID from a valid JWT token
        public ObjectId? GetEmployeeIdFromToken(string token, string userEmail)
        {
            var principal = ValidateToken(token, userEmail);
            if (principal == null) return null; // Return null if token is invalid

            // Extract employeeID claim from token
            var employeeIdClaim = principal.FindFirst("EmployeeID")?.Value;

            return employeeIdClaim != null && ObjectId.TryParse(employeeIdClaim, out var employeeId)
                ? employeeId
                : (ObjectId?)null; // Return the employeeID as an ObjectId or null if invalid
        }
    }
}

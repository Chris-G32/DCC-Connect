using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using API.Models;

namespace API.Services
{
    // Interface for AuthValidatorService providing JWT validation
    public interface IAuthValidatorService
    {
        bool ValidateToken(string token, User user);
    }

    public class AuthValidatorService : IAuthValidatorService
    {
        private readonly ILogger<AuthValidatorService> _logger;

        // Constructor initializes logger
        public AuthValidatorService(ILogger<AuthValidatorService> logger)
        {
            _logger = logger;
        }

        /*
        /// Validates a JWT token using the provided user's JWT secret.
        /// <param name="token">The JWT token to validate.</param>
        /// <param name="user">The User object containing the JWTSecret for validation.</param>
        /// <returns>True if the token is valid, otherwise false.</returns>
        */
        public bool ValidateToken(string token, User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(user.JWTSecret); // Convert the JWT secret to a byte array

            try
            {
                // Attempt to validate the token with specified parameters
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // Require valid signing key
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Use the User's JWTSecret as signing key
                    ValidateIssuer = false, // Skip issuer validation
                    ValidateAudience = false, // Skip audience validation
                    ClockSkew = TimeSpan.Zero // Set clock skew to zero for strict time validation
                }, out _); // Discards the validated token

                // Log successful validation for tracking
                _logger.LogInformation("Token validation successful for user {Email}.", user.Email);
                return true;
            }
            catch (Exception ex)
            {
                // Log validation failure details with exception message
                _logger.LogWarning("Token validation failed for user {Email}: {Exception}", user.Email, ex.Message);
                return false;
            }
        }
    }
}

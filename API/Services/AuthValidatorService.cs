using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using API.Models;

namespace API.Services
{
    public class AuthValidatorService
    {
        private readonly ILogger<AuthValidatorService> _logger;

        public AuthValidatorService(ILogger<AuthValidatorService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validates a JWT token using the provided user's JWT secret.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <param name="user">The User object containing the JWTSecret for validation.</param>
        /// <returns>True if the token is valid, otherwise false.</returns>
        public bool ValidateToken(string token, User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(user.JWTSecret); // Convert the JWT secret to a byte array

            try
            {
                // Attempt to validate the token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Validate using User's JWTSecret
                    ValidateIssuer = false, // Skip issuer validation
                    ValidateAudience = false, // Skip audience validation
                    ClockSkew = TimeSpan.Zero // No tolerance for clock skew
                }, out _); // Discards the validated token

                // Log successful validation
                _logger.LogInformation("Token validation successful for user {Email}.", user.Email);
                return true;
            }
            catch (Exception ex)
            {
                // Log any validation failures
                _logger.LogWarning("Token validation failed for user {Email}: {Exception}", user.Email, ex.Message);
                return false;
            }
        }
    }
}

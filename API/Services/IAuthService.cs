using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using API.Models;

namespace API.Services
{
    // Interface for AuthService providing authentication and JWT generation
    public interface IAuthService
    {
        string? AuthenticateAndGenerateToken(User user, string code);
    }

    public class AuthService : IAuthService
    {
        private readonly string _jwtSecret; // Secret key for signing JWT token, created at User creation
        private readonly int _jwtExpirationMinutes; // Token expiration time in minutes
        private readonly EmailService _emailService; // Service to handle email-related actions

        // Constructor initializes JWT secret, expiration time, and email service
        public AuthService(User user, int jwtExpirationMinutes, EmailService emailService)
        {
            _jwtSecret = user.JWTSecret;
            _jwtExpirationMinutes = jwtExpirationMinutes;
            _emailService = emailService;
        }

        /*
        /// Validates the 2FA code and generates a JWT token if the code is correct.
        /// <param name="user">The user object containing claims information.</param>
        /// <param name="code">The 2FA code to validate.</param>
        /// <returns>A JWT token string if authentication is successful, otherwise null.</returns>
        */
        public string? AuthenticateAndGenerateToken(User user, string code)
        {
            // Check if the 2FA code is valid
            if (!_emailService.ValidateTwoFactorCode(user.Email, code))
            {
                return null; // Return null if the code is invalid
            }

            // Create JWT token with user's claims (email, name, role)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenAuth = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] // Add user details as claims
                {
                    new Claim("EmployeeID", user.EmployeeID?.ToString() ?? string.Empty) // Add employeeID as a custom claim
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes), // Set token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Secure token signing
            };

            var token = tokenHandler.CreateToken(tokenAuth); // Generate the token
            return tokenHandler.WriteToken(token); // Return the JWT token as a string
        }
    }
}

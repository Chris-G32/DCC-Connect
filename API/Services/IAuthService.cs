using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using API.Models;
using MongoDB.Driver;

namespace API.Services
{
    public interface IAuthService
    {
        string AuthenticateAndGenerateToken(string email, string code);
    }

    public class AuthService : IAuthService
    {
        private readonly ICollectionsProvider _collectionsProvider; // For accessing MongoDB collections
        private readonly IEmailService _emailService; // Service to handle email-related actions
        private const int JwtExpirationMinutes = 60; // JWT token expiration time (constant)

        public AuthService(ICollectionsProvider collectionsProvider, IEmailService emailService)
        {
            _collectionsProvider = collectionsProvider ?? throw new ArgumentNullException(nameof(collectionsProvider));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Validates the 2FA code and generates a JWT token if the code is correct.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The 2FA code to validate.</param>
        /// <returns>A JWT token string if authentication is successful, otherwise null.</returns>
        public string AuthenticateAndGenerateToken(string email, string code)
        {
            // Fetch user from the MongoDB collection using email
            var user = _collectionsProvider.Users.Find(e => e.Email == email).FirstOrDefault();

            if (user == null || !_emailService.ValidateTwoFactorCode(email, code))
            {
                throw new ArgumentException("Invalid email or 2FA code.");
            }

            if (string.IsNullOrEmpty(user.JWTSecret))
            {
                throw new InvalidOperationException("JWT secret is not set for the user.");
            }

            // Create JWT token with user's claims (e.g., EmployeeID)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(user.JWTSecret); // Get the JWTSecret from the user for signing the token

            var tokenAuth = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim("EmployeeID", user.EmployeeID?.ToString() ?? string.Empty) // Add custom claim for EmployeeID
        }),
                Expires = DateTime.UtcNow.AddMinutes(JwtExpirationMinutes), // Set token expiration using the constant
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Secure token signing
            };

            var token = tokenHandler.CreateToken(tokenAuth); // Generate the token
            return tokenHandler.WriteToken(token); // Return the JWT token as a string
        }
    }
}

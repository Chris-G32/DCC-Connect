using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using API.Models;

namespace API.Services
{
    /// <summary>
    /// Service to generate JWT auth tokens with user-specific claims for authentication.
    /// </summary>
    public class AuthService
    {
        private readonly string _authSecret; // Secret key to secure the JWT

        public AuthService(string authSecret)
        {
            _authSecret = authSecret;
        }

        /// <summary>
        /// Generates a JWT token with claims for FirstName, LastName, and Role.
        /// </summary>
        /// <param name="user">The user object containing details for the token</param>
        /// <returns>A JWT token string</returns>
        public string GenerateToken(User user)
        {
            // Claims are pieces of information about the user, added to the token for future use
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email), // Email for user identification
                new Claim("FirstName", user.FirstName),  // First name as custom claim
                new Claim("LastName", user.LastName),    // Last name as custom claim
                new Claim(ClaimTypes.Role, RoleConversions.ToString(user.Role)) // Role for access control
            };

            // Key and credentials to secure the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token with claims, expiration, and credentials
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // 1-hour token validity
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token); // Serialize to string
        }
    }
}

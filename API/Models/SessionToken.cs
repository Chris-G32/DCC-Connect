using System;
using System.Security.Cryptography;
using System.Text;

namespace API.Models
{
    public class SessionToken : MongoObject
    {
        // Unique session token
        public string Token { get; private set; }

        // Associated user data (replace `object` with a specific type if needed)
        public String UserEmail { get; set; }

        // Expiration timestamp for the session
        public DateTime Expiration { get; private set; }

        // Default constructor
        public SessionToken(String userData)
        {
            UserEmail = userData;
            Token = GenerateToken();
            Expiration = DateTime.UtcNow.AddHours(1); // Set expiration to one hour from now
        }

        // Generates a random token string
        private static string GenerateToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var tokenBytes = new byte[32]; // 256-bit token
                rng.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes);
            }
        }

        // Helper to check if the session is still valid
        public bool IsValid()
        {
            return DateTime.UtcNow <= Expiration;
        }

        // Extends the session by a specified duration
        public void ExtendSession(TimeSpan duration)
        {
            Expiration = Expiration.Add(duration);
        }
    }
}

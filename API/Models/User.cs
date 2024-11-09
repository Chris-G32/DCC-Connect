using System;
using System.Security.Cryptography;
using System.Text;

namespace API.Models
{
    /// <summary>
    /// User class representing application users, storing personal and role-based data.
    /// </summary>
    public class User : MongoObject
    {
        // First name of the user, used for personalized interactions and JWT claims
        public string FirstName { get; set; }

        // Last name of the user, included in JWT claims for personalization
        public string LastName { get; set; }

        // Email address used for login and two-factor authentication
        public string Email { get; set; }

        // User's password hash, validated on login
        public string PasswordHash { get; set; } // Only store the hashed password

        // Role defines user's access level, stored in JWT for role-based permissions
        public Role Role { get; set; }

        /// <summary>
        /// Hashes the provided password using a secure algorithm and stores the hash.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        public void SetPassword(string password)
        {
            // Example using SHA256 - consider using a stronger hashing method like bcrypt
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                PasswordHash = Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Verifies a password against the stored hash.
        /// </summary>
        /// <param name="password">The plain-text password to verify.</param>
        /// <returns>True if the password is valid, false otherwise.</returns>
        public bool VerifyPassword(string password)
        {
            // Hash the input password and compare with the stored hash
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return PasswordHash == Convert.ToBase64String(hash);
            }
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace API.Models
{
    public class User : MongoObject
    {
        // First name for personalization and JWT claims
        public string FirstName { get; set; }

        // Last name for personalization and JWT claims
        public string LastName { get; set; }

        // Email for login and 2FA
        public string Email { get; set; }

        // Hashed password, stored for auth
        public string PasswordHash { get; set; }

        // Role for defining user access level
        public Role Role { get; set; }

        // Hashes the password and stores the hash
        public void SetPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                PasswordHash = Convert.ToBase64String(hash);
            }
        }

        // Verifies the input password by comparing hashes
        public bool VerifyPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return PasswordHash == Convert.ToBase64String(hash);
            }
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;
using MimeKit.Encodings;
using MongoDB.Bson;

namespace API.Models
{
    public class User : MongoObject
    {

        // Email for login and 2FA
        public string Email { get; set; }

        // Hashed password, stored for auth
        public string PasswordHash { get; set; }

        // Secret Key for JWT Auth token and validation, each one is uniquely generated
        public string JWTSecret { get; set; }

        // Role for defining user access level
        public Role UserRole { get; set; }

        // Used for JWT auth and referencing
        public ObjectId? EmployeeID { get; set; } // Made nullable

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
                // Convert the plain text password into bytes
                var bytes = Encoding.UTF8.GetBytes(password);

                // Compute the hash of the password
                var hash = sha256.ComputeHash(bytes);

                // Compare the computed hash with the stored password hash
                return PasswordHash == Convert.ToBase64String(hash);
            }
        }
    }
}

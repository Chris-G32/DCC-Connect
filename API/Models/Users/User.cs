using API.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace API.Models.Users;
public interface IUser : IUserInfo
{
    string PasswordHash { get; set; }
}
/// <summary>
/// This is the user that should be stored in the database. Contains all information, including password hash and any other information only pertinent to backend. Do NOT return this object to the client.
/// </summary>
public class User : UserInfo, IUser
{
    public User() { }
    [SetsRequiredMembers]
    public User(UserRegistrationInfo registrationInfo,string password)
    {
        FirstName = registrationInfo.FirstName;
        LastName = registrationInfo.LastName;
        PhoneNumber = registrationInfo.PhoneNumber;
        EmployeeRole = registrationInfo.EmployeeRole;
        Email = registrationInfo.Email;
        PasswordHash = HashPassword(password);
    }
    // Hashed password, stored for auth
    public required string PasswordHash { get; set; }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
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
        using var sha256 = SHA256.Create();
        // Convert the plain text password into bytes
        var bytes = Encoding.UTF8.GetBytes(password);

        // Compute the hash of the password
        var hash = sha256.ComputeHash(bytes);

        // Compare the computed hash with the stored password hash
        return PasswordHash == Convert.ToBase64String(hash);
    }
}

using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace API.Services.Authentication;

public interface IPasswordService
{
    /// <summary>
    /// Validates a password against a hash.
    /// </summary>
    /// <param name="password">Plaintext password to check</param>
    /// <param name="passwordHash">Hash to check against</param>
    /// <returns>True if the hash of the password matches, false otherwise.</returns>
    bool VerifyPassword(string password, string passwordHash);
    /// <summary>
    /// Generates a random password of a given length.
    /// </summary>
    /// <param name="length">Length of password to generate</param>
    /// <returns> A random password of the given length.</returns>
    string GenerateRandomPassword(int length);
    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password"> The password plaintext to hash</param>
    /// <returns> The hashed password.</returns>
    string HashPassword(string password);
    /// <summary>
    /// Takes a password and checks that it is a valid password
    /// </summary>
    /// <param name="password">Password to check</param>
    /// <returns> A tuple contianing if the password is valid, and a detail if it is not valid of why.</returns>
    Tuple<bool, string?> IsValidPassword(string password);
}
public class PasswordService : IPasswordService
{
    private readonly int _minPasswordLength = 8;
    private ReadOnlySpan<char> ValidCharacters => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+".AsSpan();
    ///<inheritdoc/>
    public bool VerifyPassword(string password, string passwordHash)
    {
        using var sha256 = SHA256.Create();
        // Convert the plain text password into bytes
        var bytes = Encoding.UTF8.GetBytes(password);

        // Compute the hash of the password
        var hash = sha256.ComputeHash(bytes);

        // Compare the computed hash with the stored password hash
        return passwordHash == Convert.ToBase64String(hash);

    }
    ///<inheritdoc/>
    public string GenerateRandomPassword(int length)
    {
        var validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+".AsSpan();
        if (length <= 0)
        {
            throw new ArgumentException("Password length must be greater than 0.");
        }
        var password = new string(RandomNumberGenerator.GetItems(validCharacters, length));

        return password;
    }
    ///<inheritdoc/>
    public string HashPassword(string password)
    {
        if(password.Length <1)
        {
            throw new ArgumentException("Password is empty.");
        }
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    ///<inheritdoc/>
    public Tuple<bool, string?> IsValidPassword(string password)
    {
        if (password.Length < _minPasswordLength)
        {
            return Tuple.Create<bool, string?>(false, "Password is too short.");
        }
        var invalidCharacters = password.Where((character) => !ValidCharacters.Contains(character)).ToList();
        if (invalidCharacters != null)
        {
            return Tuple.Create<bool, string?>(false, $"Password contains invalid characters: {string.Join(",", invalidCharacters)}");
        }
        return Tuple.Create<bool, string?>(true, null);
    }
}

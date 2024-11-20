using API.Models;
using MongoDB.Bson;
using System.Security.Cryptography;
using System.Text;

namespace API.Services;

public interface IUserRegisterService
{
    Task<User> RegisterUser(UserRegistrationInfo userRegistrationInfo);
}
public class UserRegisterService(ILogger<UserRegisterService> logger, ICollectionsProvider cp, IEmailService emailService) : IUserRegisterService
{
    private readonly ILogger<UserRegisterService> _logger = logger;
    private readonly ICollectionsProvider _collectionsProvider = cp;
    private readonly IEmailService _emailService = emailService;
    private const string ValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
    public static string GenerateSecurePassword(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Password length must be greater than 0.");
        }
        var password = new StringBuilder(); using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[4]; while (password.Length < length)
            {
                rng.GetBytes(randomBytes);
                uint randomValue = BitConverter.ToUInt32(randomBytes, 0);
                password.Append(ValidCharacters[(int)(randomValue % (uint)ValidCharacters.Length)]);
            }
        }
        return password.ToString();
    }
    public async Task<User> RegisterUser(UserRegistrationInfo userRegistrationInfo)
    {
        var password = GenerateSecurePassword(24);
        var employee = new Employee()
        {
            Id = ObjectId.GenerateNewId(),
            FirstName = userRegistrationInfo.FirstName,
            LastName = userRegistrationInfo.LastName,
            PhoneNumber = userRegistrationInfo.PhoneNumber,
            EmployeeRole = userRegistrationInfo.EmployeeRole
        };
        _collectionsProvider.Employees.InsertOne(employee);
        var user = new User();
        user.Id = ObjectId.GenerateNewId();
        user.Email = userRegistrationInfo.Email;
        user.SetPassword(password);
        user.EmployeeID = employee.Id;

        // Generate a unique JWT Secret
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] secretBytes = new byte[32];
            rng.GetBytes(secretBytes);
            user.JWTSecret = Convert.ToBase64String(secretBytes);
        }

        _collectionsProvider.Users.InsertOne(user);
        await _emailService.SendEmailAsync(user.Email, "Welcome to the company!", $"Your password is: {password}. Please reset it upon signing in.");
        return user;
    }
}
using API.Models;
using MongoDB.Bson;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver;
using API.Errors;
using API.Models.Users;
using System.Collections.Immutable;
using API.Services.Authentication;
namespace API.Services;

public interface IUserRegisterService
{
    Task<User> RegisterUser(UserRegistrationInfo userRegistrationInfo);
}
public class UserRegisterService(ILogger<UserRegisterService> logger, ICollectionsProvider cp, IEmailService emailService, IPasswordService passwordService) : IUserRegisterService
{
    private readonly ILogger<UserRegisterService> _logger = logger;
    private readonly ICollectionsProvider _collectionsProvider = cp;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordService _passwordService = passwordService;
    public async Task<User> RegisterUser(UserRegistrationInfo userRegistrationInfo)
    {
        var password = _passwordService.GenerateRandomPassword(24);
        var user = new User(userRegistrationInfo, password);
        try
        {
            _collectionsProvider.Users.InsertOne(user);
        }
        catch (MongoWriteException e)
        {
            throw new DCCApiException("User with this email already exists.", e);
        }

        await _emailService.SendEmailAsync(user.Email, "Welcome to the company!", $"Your password is: {password}. Please reset it upon signing in.");
        return user;
    }
}
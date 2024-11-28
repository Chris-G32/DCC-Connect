using Amazon.SecurityToken.Model;
using API.Errors;
using API.Models.SignIn;
using API.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace API.Services.Authentication;

public interface ILoginService
{
    /// <summary>
    /// Attempts to start a session for the given credentials. Returns true on valid credentials, false otherwise.
    /// </summary>
    /// <param name="credentials">Credentials to attempt to start a session for</param>
    /// <returns>The JWT token for the session</returns>
    public bool StartSession(LoginRequest credentials);
    /// <summary>
    /// Validates MFA code to finalize session initialization.
    /// </summary>
    /// <param name="credentials">Credentials containing the email and the MFA code.</param>
    /// <returns>JWT Token for the successful sign in</returns>
    ///<exception cref="DCCApiException">Thrown when the MFA code is invalid or the user does not exist.</exception>"
    public string ValidateMFA(LoginRequest credentials);
}
public class LoginService(ILogger<LoginService> logger, IUserService userService, IPasswordService passwordService, IMFAService mfaService, IJwtTokenGenerator tokenGenerator) : ILoginService
{
    private readonly ILogger<LoginService> _logger = logger;
    private readonly IUserService _userService = userService;
    private readonly IMFAService _mfaService = mfaService;
    private readonly IJwtTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IPasswordService _passwordService = passwordService;
    public bool StartSession(LoginRequest credentials)
    {
        User? user = _userService.GetUserByEmail(credentials.Email);
        if (user == null) { return false; }

        bool validPassword = _passwordService.VerifyPassword(credentials.Credential, user.PasswordHash);
        if (!validPassword)
        {
            return false;
        }
        _mfaService.SendMFACode(credentials.Email);
        return true;
    }

    public string ValidateMFA(LoginRequest credentials)
    {
        if (!_mfaService.ValidateCode(credentials.Email, credentials.Credential))
        {
            throw new DCCApiException("Failed MFA validation");
        }
        //This should never be null if the MFA code is valid
        var user = _userService.GetUserByEmail(credentials.Email);
        if (user == null)
        {
            _logger.LogCritical("Received valid MFA code for non existent user");
            throw new DCCApiException("Failed MFA validation. User does not exist.");
        }
        return _tokenGenerator.GenerateToken(_userService.GetUserByEmail(credentials.Email) ?? throw new DCCApiException("User does not ex"));
    }
}
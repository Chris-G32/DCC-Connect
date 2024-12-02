using API.Errors;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace API.Services.Authentication;

public interface IMFAService
{
    bool ValidateCode(string email, string code);
    void SendMFACode(string email);
}
struct MFACodeAndExpiry
{
    public string Code { get; set; }
    public DateTime Expiry { get; set; }
}
public class MFAService : IMFAService
{
    private const int MFA_CODE_LENGTH = 6;
    private static Dictionary<string, MFACodeAndExpiry> _twoFactorCodes = new Dictionary<string, MFACodeAndExpiry>();
    private readonly ILogger<MFAService> _logger;
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    public MFAService(ILogger<MFAService> logger, IEmailService emailService, IUserService userService)
    {
        Task.Run(DeleteOldCodes);
        _logger = logger;
        _emailService = emailService;
        _userService = userService;
    }
    private async Task DeleteOldCodes()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromMinutes(5));
            foreach (var code in _twoFactorCodes)
            {
                if (code.Value.Expiry < DateTime.Now)
                {
                    _twoFactorCodes.Remove(code.Key);
                }
            }
        }
    }
    private string GenerateMFACode()
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] buffer = new byte[4];  // 4 bytes gives us a 32-bit number (can generate a larger range)
        rng.GetBytes(buffer);

        // Convert the bytes into a positive integer
        int randomNumber = Math.Abs(BitConverter.ToInt32(buffer, 0));

        // Scale the number to ensure it's always a 6-digit number (100000 to 999999)
        int mfaCode = randomNumber % 900000 + 100000;

        // Return the 6-digit MFA code as a string
        return mfaCode.ToString("D6"); // D6 formats it to always have 6 digits
    }

    public void SendMFACode([EmailAddress] string email)
    {
        _ = _userService.GetUserByEmail(email) ?? throw new DCCApiException("User does not exist!");
        var code = GenerateMFACode();
        var expiryTime = 2;
        _twoFactorCodes[email] = new MFACodeAndExpiry { Code = code, Expiry = DateTime.Now.AddMinutes(expiryTime) };
        _emailService.SendEmailAsync(email, "Your MFA Code", $"Your MFA Code is: {code}.\nIt will expire in {expiryTime} minutes");
    }
    /// <summary>
    /// Validates an MFA Code for a given email.
    /// </summary>
    /// <param name="email">Email that code was sent to</param>
    /// <param name="code">Mfa code received by user</param>
    /// <returns> True when MFA code matches stored email. 
    /// False if email is not associated with a user or the code is expired/invalid.</returns>
    /// <exception cref="DCCApiException"> When an invalid formatted MFA code is provided</exception>
    public bool ValidateCode([EmailAddress] string email, string code)
    {
        if (_userService.GetUserByEmail(email) == null) { return false; }

        if (code.Length != MFA_CODE_LENGTH)
        {
            throw new DCCApiException("Invalid MFA Code.");
        }
        var result = _twoFactorCodes.TryGetValue(email, out var mfaCode);
        return result && mfaCode.Code == code && mfaCode.Expiry > DateTime.Now;
    }
}
using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class UserRegistrationInfo
{

    // Email for login and 2FA
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
    public string? EmployeeID { get; set; }
}

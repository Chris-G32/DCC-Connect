using System.ComponentModel.DataAnnotations;

namespace API.Models.Users;

/// <summary>
/// Exists mainly so API doesn't accept an object id in registration
/// </summary>
public class UserRegistrationInfo: IUserInfo
{

    // Email for login and 2FA
    [EmailAddress]
    public string Email { get; set; }
    /// <summary>
    /// First name of the employee
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// Last name of the employee
    /// </summary>
    public string LastName { get; set; }
    /// <summary>
    /// Contact for the employee
    /// </summary>
    [Phone]
    public string PhoneNumber { get; set; }
    public string EmployeeRole { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace API.Models.Users;

public interface IUserInfo: IEmployeeInfo
{
    string PhoneNumber { get; set; }
    string Email { get; set; }
}
/// <summary>
/// Information to return to frontend about a user. Basically all data but password hash.
/// </summary>
public class UserInfo:EmployeeInfo,IUserInfo
{
    [Phone]
    public required string PhoneNumber { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
}
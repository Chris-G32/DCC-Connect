using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
    public UserInfo() { }

    [SetsRequiredMembers]
    public UserInfo(IUserInfo userInfo)
    {
        FirstName = userInfo.FirstName;
        LastName = userInfo.LastName;
        EmployeeRole = userInfo.EmployeeRole;
        PhoneNumber = userInfo.PhoneNumber;
        Email = userInfo.Email;
    }
    [SetsRequiredMembers]
    public UserInfo(UserInfo userInfo):this((IUserInfo)userInfo)
    {
        Id = userInfo.Id;
    }
    [Phone]
    public required string PhoneNumber { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
}
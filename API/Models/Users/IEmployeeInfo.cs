using API.Constants;
using API.Errors;
using System.Diagnostics.CodeAnalysis;

namespace API.Models.Users;

public interface IEmployeeInfo
{
    string FirstName { get; set; }
    string LastName { get; set; }
    string EmployeeRole { get; set; }
}
/// <summary>
/// This is used to store data related to the employee that should be accessible to all users. Not meant to be stored in the database.
/// </summary>
public class EmployeeInfo : MongoObject, IEmployeeInfo
{
    public EmployeeInfo() { }
    [SetsRequiredMembers]
    public EmployeeInfo(EmployeeInfo employeeInfo)
    {
        Id = employeeInfo.Id;
        FirstName = employeeInfo.FirstName;
        LastName = employeeInfo.LastName;
        EmployeeRole = employeeInfo.EmployeeRole;
    }
    public required string FirstName { get ; set ; }
    public required string LastName { get; set; }
    private string? _employeeRole;
    public required string EmployeeRole
    {
        get { return _employeeRole; }
        set
        {
            if (!RoleConstants.ValidRoles.Contains(value))
            {
                throw new DCCApiException($"\"{value}\" is not a valid employee role.");
            }
            _employeeRole = value;
        }
    }
}

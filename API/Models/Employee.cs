using API.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models;

public class Employee : MongoObject
{
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
    private string _employeeRole;
    public string EmployeeRole
    {
        get { return _employeeRole; }
        set
        {
            if (!RoleConstants.ValidRoles.Contains(value))
            {
                throw new InvalidDataException($"\"{value}\" is not a valid role.");
            }
            _employeeRole = value;
        }
    }
}

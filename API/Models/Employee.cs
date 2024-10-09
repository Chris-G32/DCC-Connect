using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Employee:MongoObject
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

}

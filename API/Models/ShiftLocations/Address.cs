using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace API.Models.ShiftLocations;

public class Address
{

    /// <summary>
    /// Redundant method bc i cant consistently remember postal code vs zip code
    /// </summary>    
    [BsonIgnore]
    [JsonIgnore]
    public string ZipCode { get => PostalCode; }
    public string StreetAddress { get; set; }  // Street address (e.g., "123 Main St")
    public string City { get; set; }           // City (e.g., "Springfield")
    public string State { get; set; }          // State (e.g., "IL")
    public string PostalCode { get; set; }     // Postal Code (e.g., "62701")
    public string Country { get; set; }        // Country (optional)

    // Optional: For more complex structures like apartment or suite numbers
    public string? ApartmentNumber { get; set; } // (e.g., "Apt 101" or "Suite 500")

    // Constructor to initialize the address
    public Address(string streetAddress, string city, string state, string postalCode, string country = "USA", string apartmentNumber = "")
    {
        StreetAddress = streetAddress;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        ApartmentNumber = apartmentNumber;
    }

    // Override ToString for a formatted address output
    public override string ToString()
    {
        return $"{StreetAddress}, {ApartmentNumber}, {City}, {State} {PostalCode}, {Country}";
    }
}

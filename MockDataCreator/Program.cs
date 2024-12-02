using API.Config;
using API.Constants;
using API.Models;
using API.Models.QueryOptions;
using API.Models.ShiftLocations;
using API.Models.Shifts;
using API.Models.Users;
using API.Services;
using API.Services.Authentication;
using AutoFixture;
using Bogus;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
var fixture = new Fixture();
var faker = new Faker();

var locations = new List<ShiftLocation>();
for (int i = 0; i < 10; i++)
{
    var shiftAddress = fixture.Build<Address>()
        .With(a => a.City, faker.Address.City())
        .With(a => a.State, faker.Address.State())
        .With(a => a.StreetAddress, faker.Address.StreetAddress())
        .With(a => a.PostalCode, faker.Address.ZipCode())
        .With(a => a.Country, "United States")
        .With(a => a.ApartmentNumber, faker.Address.SecondaryAddress().OrNull(faker, .75f))
        .Create();
    var location = new ShiftLocation
    {
        PatientName = faker.Name.FullName(),
        StreetAddress = shiftAddress,
        Id = ObjectId.GenerateNewId()
    };
    locations.Add(location);
}
IPasswordService pwService= new PasswordService();
// Generate 50 mock Employee records
var employees = new List<User>();
for (int i = 0; i < 50; i++)
{
    var firstName = faker.Name.FirstName();
    var lastName = faker.Name.LastName();
    var phoneNumber = faker.Phone.PhoneNumber("(###) ###-####");
    string role = faker.PickRandom(RoleConstants.ValidRoles.ToArray());
    var registrationInfo = fixture.Build<UserRegistrationInfo>()
        .With(e => e.FirstName, firstName)
        .With(e => e.LastName, lastName)
        .With(e => e.PhoneNumber, phoneNumber)
        .With(e => e.EmployeeRole, role)
        .With(e => e.Email, faker.Internet.Email(firstName, lastName))
        .Create();
    var password = pwService.HashPassword(pwService.GenerateRandomPassword(12));
    var user = new User(registrationInfo, password);
    user.Id = ObjectId.GenerateNewId();
    employees.Add(user);
}

// Generate 50 mock Shift records
var shifts = new List<Shift>();
var now = DateTime.UtcNow;

// Set the start time for shifts (at least 12 hours in the future)
var startTime = now.AddHours(12);
var endTime = startTime.AddMonths(1);

for (int i = 0; i < 500; i++)
{
    // Generate random shift start and end times
    var shiftStart = faker.Date.Between(startTime, endTime);
    var shiftEnd = shiftStart.AddHours(faker.Random.Double(6, 16)); // Random length between 1 and 16 hours

    var timeRange = new TimeRange(shiftStart, shiftEnd); // Assuming TimeRange has a constructor that accepts start and end times
    var shouldAssign = faker.Random.Float() > 0.3;
    var role = faker.PickRandom(RoleConstants.ValidRoles.ToArray());
    ObjectId? employeeId = null;
    if (shouldAssign)
    {
        var matchingRoles = employees.Where(employee => employee.EmployeeRole == role).ToList();
        employeeId = faker.PickRandom(matchingRoles).Id;
    }
    var shift = new Shift
    {
        ShiftPeriod = timeRange,
        Location = faker.PickRandom(locations).Id ?? throw new Exception("Object id should be set for the location"),
        RequiredRole = role, // Assuming Role is an enum or class you have defined
        EmployeeID = employeeId,
    };

    shifts.Add(shift);
}

// Insert the generated shifts into the database


MongoDBSettings settings = new();
settings.Port = 27017;
settings.URL = "localhost";
settings.Database = "dcc-connect-db";

var db = new MongoClient(settings.GetClientSettings()).GetDatabase(settings.Database);
// comment these lines out to make more new data instead of dropping old data
db.DropCollection(CollectionConstants.LocationsCollection);
db.DropCollection(CollectionConstants.ShiftsCollection);
db.DropCollection(CollectionConstants.UsersCollection);

db.GetCollection<ShiftLocation>(CollectionConstants.LocationsCollection).InsertMany(locations);
db.GetCollection<Shift>(CollectionConstants.ShiftsCollection).InsertMany(shifts);
db.GetCollection<User>(CollectionConstants.UsersCollection).InsertMany(employees);
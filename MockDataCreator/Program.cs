using API.Config;
using API.Constants;
using API.Models;
using API.Models.QueryOptions;
using API.Models.Shifts;
using API.Services;
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

// Generate 50 mock Employee records
var employees = new List<Employee>();
for (int i = 0; i < 500; i++)
{
    var firstName = faker.Name.FirstName();
    var lastName = faker.Name.LastName();
    var phoneNumber = faker.Phone.PhoneNumber("(###) ###-####");

    var employee = fixture.Build<Employee>()
        .With(e => e.FirstName, firstName)
        .With(e => e.LastName, lastName)
        .With(e => e.PhoneNumber, phoneNumber)
        .With(e => e.EmployeeRole, faker.PickRandom(RoleConstants.ValidRoles.ToArray()))
        .With(e => e.Id,ObjectId.GenerateNewId()) // Exclude Id property from being set
        .Create();

    employees.Add(employee);
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
    var shouldAssign=faker.Random.Float()>0.3;
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
        Location = faker.Address.FullAddress(),
        RequiredRole = role, // Assuming Role is an enum or class you have defined
        EmployeeID= employeeId,
    };

    shifts.Add(shift);
}

// Insert the generated shifts into the database


MongoDBSettings settings = new();
settings.Port = 27017;
settings.URL = "localhost";
settings.Database = "dcc-connect-db";

var db =new MongoClient(settings.GetClientSettings()).GetDatabase(settings.Database);
 
db.GetCollection<Shift>("shifts").InsertMany(shifts);
db.GetCollection<Employee>("employees").InsertMany(employees);
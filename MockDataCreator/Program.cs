using API.Config;
using API.Models;
using API.Services;
using AutoFixture;
using Bogus;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
var fixture = new Fixture();
var faker = new Faker();

// Generate 50 mock Employee records
var employees = new List<Employee>();
for (int i = 0; i < 50; i++)
{
    var firstName = faker.Name.FirstName();
    var lastName = faker.Name.LastName();
    var phoneNumber = faker.Phone.PhoneNumber("(###) ###-####");

    var employee = fixture.Build<Employee>()
        .With(e => e.FirstName, firstName)
        .With(e => e.LastName, lastName)
        .With(e => e.PhoneNumber, phoneNumber)
        .Create();

    employees.Add(employee);
}

//// Output the generated employees
//foreach (var employee in employees)
//{
//    Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}, Email: {employee.Email}, Phone: {employee.PhoneNumber}");
//}
MongoDBSettings settings = new();
settings.Port = 27017;
settings.URL = "localhost";
settings.Database = "dcc-connect-db";

var db =new MongoClient(settings.GetClientSettings()).GetDatabase(settings.Database);

db.GetCollection<Employee>("employees").InsertMany(employees);



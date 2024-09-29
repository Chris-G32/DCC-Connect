using API.Models;
using API.Services;
using API.Config;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using API.Constants;
using System.Linq.Expressions;
using System;

namespace API.Database;

public interface IDBInit
{
    void Initialize();
}
public class MongoDBInitializer(IConfiguration config) : IDBInit
{
    private readonly MongoDBSettings _settings = config.GetRequiredSection(DatabaseConstants.DatabaseSettingsSection).Get<MongoDBSettings>() ?? throw new Exception("MongoDB settings not found in appsettings.json");
    IMongoDatabase _database;
    private bool initialized = false;
    void createUniqueIndex<T>(Expression<Func<T, object>> fieldSpecifier, IMongoCollection<T> collection) where T : class
    {
        var indexKeysDefinition = Builders<T>.IndexKeys.Ascending(fieldSpecifier);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<T>(indexKeysDefinition, indexOptions);
        collection.Indexes.CreateOne(indexModel);
    }

    private MongoClientSettings GetClientSettings()
    {
        var clientSettings = new MongoClientSettings
        {
            Server = new MongoServerAddress(_settings.URL, _settings.Port)
        };
        if (_settings.Authentication != null)
        {
            clientSettings.Credential = MongoCredential.CreateCredential(_settings.Database, _settings.Authentication.Username, _settings.Authentication.Password);
        }
        return clientSettings;
    }
    public void Initialize()
    {
        var settings = GetClientSettings();
        var client = new MongoClient(settings);
        _database = client.GetDatabase(DatabaseConstants.DatabaseName);

        //Create Employee Collection
        try
        {
            _database.CreateCollection("employees");
            // A given email can only be assigned to one employee
            createUniqueIndex(e => e.Email, _database.GetCollection<Employee>("employees"));
        }
        catch (Exception ex) { }

        try
        {
            //Create Shift Collection
            _database.CreateCollection("shifts");
            _database.CreateCollection("shifts.assignments");
            // A given shift can only be assigned to one employee
            createUniqueIndex(assignment => assignment.ShiftID, _database.GetCollection<ShiftAssignment>("shifts.assignments"));
        }
        catch (Exception ex) { }
        initialized = true;
    }
}

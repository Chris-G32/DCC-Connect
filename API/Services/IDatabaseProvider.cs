using API.Models;
using MongoDB.Driver;

namespace API.Services;

public interface IDatabaseProvider
{
    IMongoDatabase Database { get; }
}

public class DatabaseProvider : IDatabaseProvider
{
    private readonly IMongoDatabase _database;

    public DatabaseProvider(string connectionString= "mongodb://localhost:27017")
    {
        var client = new MongoClient(connectionString);

        _database = client.GetDatabase("dcc-connect-db");
        
        // Create a unique index on the Email field
        var indexKeysDefinition = Builders<Employee>.IndexKeys.Ascending(e => e.Email);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Employee>(indexKeysDefinition, indexOptions);
        _database.GetCollection<Employee>("employees").Indexes.CreateOne(indexModel);


    }

    public IMongoDatabase Database => _database;
}
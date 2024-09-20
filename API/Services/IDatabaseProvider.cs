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
    }

    public IMongoDatabase Database => _database;
}
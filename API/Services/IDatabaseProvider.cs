using API.Constants;
using API.Database;
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
    private readonly IConfiguration _config;
    public DatabaseProvider(IDatabaseInitializer dbInit,IDBClientProvider clientProvider )
    {
        dbInit.InitializeDatabase();
        _database=clientProvider.Client.GetDatabase(DatabaseConstants.DatabaseName);
    }

    public IMongoDatabase Database => _database;
}
using API.Config;
using API.Constants;
using API.Models;
using MongoDB.Driver;

namespace API.Services;

public interface IDatabaseProvider
{
    IMongoDatabase Database { get; }
}

public class DatabaseProvider : IDatabaseProvider
{
    private readonly IDBClientProvider _clientProvider;
    private readonly MongoDBSettings _config;
    public DatabaseProvider(IDatabaseInitializer dbInit, IDBClientProvider clientProvider, IMongoDBSettingsProvider settingsProvider, ILogger<DatabaseProvider> loggger)
    {
        _ = Task.Run(dbInit.InitializeDatabase);
        _clientProvider = clientProvider;
        _config = settingsProvider.GetSettings();
    }

    public IMongoDatabase Database => _clientProvider.Client.GetDatabase(_config.Database);

}
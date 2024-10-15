using API.Config;
using API.Constants;
using MongoDB.Driver;
using System.Runtime;

namespace API.Services;

public interface IDBClientProvider
{
    IMongoClient Client { get; }
}
public class MongoClientProvider : IDBClientProvider
{
    private readonly MongoDBSettings _settings;
    private readonly IMongoClient _client;

    public MongoClientProvider(IConfiguration config)
    {
        _settings = config.GetRequiredSection(DatabaseConstants.DatabaseSettingsSection).Get<MongoDBSettings>() ?? throw new Exception("MongoDB settings not found in appsettings.json");
        _client = new MongoClient(_settings.GetClientSettings());
    }
    public IMongoClient Client => _client;
}
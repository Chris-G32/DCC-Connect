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

    public MongoClientProvider(IMongoDBSettingsProvider settingsProvider)
    {
        _settings = settingsProvider.GetSettings();
        _client = new MongoClient(_settings.GetClientSettings());
    }
    public IMongoClient Client => _client;
}
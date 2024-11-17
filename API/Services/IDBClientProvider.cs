using API.Config;
using API.Constants;
using MongoDB.Driver;
using System.Runtime;

namespace API.Services;

public interface IDBClientProvider
{
    IMongoClient Client { get; }
    public bool ClientConnected { get; }
}
public class MongoClientProvider : IDBClientProvider
{
    private readonly MongoDBSettings _settings;
    private IMongoClient _client;
    private readonly ILogger<MongoClientProvider> _logger;
    private async Task TryInitClientUntilSuccess()
    {

        while (true)
        {
            try
            {
                _client = new MongoClient(_settings.GetClientSettings());
                _client.ListDatabaseNames();
                ClientConnected = true;
                _logger.LogInformation("Database client initialized successfully.");
                return;
            }
            catch (TimeoutException e)
            {
                _logger.LogWarning($"Timeout when attempting to connect MongoClient with the following info URL: {_settings.URL}, Port: {_settings.Port}. Check that server is live.\n Retrying...");
            }

        }
    }
    public MongoClientProvider(IMongoDBSettingsProvider settingsProvider, ILogger<MongoClientProvider> logger)
    {
        _logger = logger;
        _settings = settingsProvider.GetSettings();
        ClientConnected = false;
        Task.Run(TryInitClientUntilSuccess);
    }

    public IMongoClient Client => _client;

    public bool ClientConnected { get; private set; }
}
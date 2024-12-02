using MongoDB.Driver;

namespace API.Config;
public class MongoDBSettings
{
    /// <summary>
    /// If specified this will be used and all other settings will be ignored.
    /// </summary>
    public string? ConnectionString { get; set; } = null;
    public string URL { get; set; }
    public int Port { get; set; }
    public DbAuth? Authentication { get; set; }
    public string Database { get; set; }
    public int? ConnectionTimeoutMS { get; set; }
    public MongoClientSettings GetClientSettings()
    {
        MongoClientSettings settings;
        if (string.IsNullOrEmpty(ConnectionString)){
            settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(URL, Port),
                ServerSelectionTimeout = TimeSpan.FromMilliseconds(ConnectionTimeoutMS ?? 30000)
            };
            if (Authentication != null)
            {
                settings.Credential = MongoCredential.CreateCredential(Database, Authentication.Username, Authentication.Password);
            }
        }
        else
        {
            settings = MongoClientSettings.FromConnectionString(ConnectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromMilliseconds(ConnectionTimeoutMS ?? 30000);
        }
        return settings;
    }
}


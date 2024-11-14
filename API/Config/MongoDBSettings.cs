using MongoDB.Driver;

namespace API.Config;
public class MongoDBSettings
{
    public string URL { get; set; }
    public int Port { get; set; }
    public DbAuth? Authentication { get; set; }
    public string Database { get; set; }
    public int? ConnectionTimeoutMS { get; set; }
    public MongoClientSettings GetClientSettings()
    {
        var settings = new MongoClientSettings
        {
            Server = new MongoServerAddress(URL, Port),
            ServerSelectionTimeout = TimeSpan.FromMilliseconds(ConnectionTimeoutMS ?? 30000)
        };
        if (Authentication != null)
        {
            settings.Credential = MongoCredential.CreateCredential(Database, Authentication.Username, Authentication.Password);
        }
        return settings;
    }
}


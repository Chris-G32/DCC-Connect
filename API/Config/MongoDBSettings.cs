namespace API.Config;
public class MongoDBSettings
{
    public string URL { get; set; }
    public int Port { get; set; }
    public DbAuth? Authentication { get; set; }
    public string Database { get; set; }
}


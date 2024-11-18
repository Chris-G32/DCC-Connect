using API.Config;
using API.Constants;

namespace API.Services;

public interface IMongoDBSettingsProvider
{
    MongoDBSettings GetSettings();
}
public class MongoDBSettingsProvider(IConfiguration config) : IMongoDBSettingsProvider
{
    private readonly MongoDBSettings _settings = config.GetRequiredSection(DatabaseConstants.DatabaseSettingsSection).Get<MongoDBSettings>() ?? throw new Exception("MongoDB settings not found in appsettings.json");

    public MongoDBSettings GetSettings()
    {
        return _settings;
    }
}

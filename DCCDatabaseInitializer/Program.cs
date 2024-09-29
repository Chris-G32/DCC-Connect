using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using DCCDatabaseInitializer.Config;

// Build configuration

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
// Load MongoDB settings
var mongoDbSettings = configuration.GetRequiredSection("MongoDB").Get<MongoDBSettings>();
if (mongoDbSettings == null)
{
    throw new Exception("MongoDB settings not found in appsettings.json");
}

var settings = new MongoClientSettings
{
    Server = new MongoServerAddress(mongoDbSettings.URL, mongoDbSettings.Port)
};
if (mongoDbSettings.Authentication != null)
{
    settings.Credential = MongoCredential.CreateCredential(mongoDbSettings.Database, mongoDbSettings.Authentication.Username, mongoDbSettings.Authentication.Password);
}
var client = new MongoClient(settings);
var db=client.GetDatabase(mongoDbSettings.Database);
// Create a unique index on the Email field

db.ListCollectionNames().ToList().ForEach(Console.WriteLine);
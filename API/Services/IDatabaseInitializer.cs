using API.Config;
using API.Constants;
using API.Models.Scheduling.Coverage;
using API.Models.Users;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Collections;
using System.Linq.Expressions;

namespace API.Services;

public interface IDatabaseInitializer
{
    void InitializeDatabase();
}

public class DatabaseInitializer(IConfiguration config, IDBClientProvider clientProvider,ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    private readonly MongoDBSettings _settings = config.GetRequiredSection(DatabaseConstants.DatabaseSettingsSection).Get<MongoDBSettings>() ?? throw new Exception("MongoDB settings not found in appsettings.json");
    private readonly IDBClientProvider _clientProvider = clientProvider;
    private readonly ILogger<DatabaseInitializer> _logger=logger;
    private List<string> _collectionNames;
    /// <summary>
    /// This ensures a field is unique between entries in a collection. If a field is not unique, an exception will be thrown when trying to insert a document with a duplicate value.
    /// No validation is needed to be done before calling this method, as it will be ignored if the index already exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldSpecifier"></param>
    /// <param name="collection"></param>
    internal string createUniqueIndex<T>(Expression<Func<T, object>> fieldSpecifier, IMongoCollection<T> collection) where T : class
    {
        var indexKeysDefinition = Builders<T>.IndexKeys.Ascending(fieldSpecifier);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<T>(indexKeysDefinition, indexOptions);
        return collection.Indexes.CreateOne(indexModel);
        
    }
    private void CreateCollectionIfNotExists(string collectionName,IMongoDatabase db)
    {
        //Create Employee Collection
        if (!_collectionNames.Contains(collectionName))
        {
            db.CreateCollection(collectionName);
            _logger.LogInformation($"Created collection {collectionName}");
            return;
        }
        _logger.LogInformation($"Collection {collectionName} already exists");
    }
    private void LogUniqueCreationSuccess(string collectionName,string field,string indexName)
    {
        _logger.LogInformation($"Creeated unique index \"{indexName}\" on collection \"{collectionName}\" for field \"{field}\".");
    }
    public void InitializeDatabase()
    {
        
        bool initSucceeded = false;
        while (!initSucceeded)
        {
            try
            {
                while (!_clientProvider.ClientConnected)
                {
                    _logger.LogWarning("Awaiting successful client connection...");
                    Thread.Sleep(3000);
                }

                _logger.LogInformation($"Attempting to initialize database {_settings.Database}...");
                var db = _clientProvider.Client.GetDatabase(_settings.Database);
                _collectionNames = db.ListCollectionNames().ToList();
                //Create Users Collection
                CreateCollectionIfNotExists(CollectionConstants.UsersCollection, db);
                createUniqueIndex(user => user.Email, db.GetCollection<User>(CollectionConstants.UsersCollection));
                //Create locations collection
                CreateCollectionIfNotExists(CollectionConstants.LocationsCollection, db);

                //Create Shift Collection
                CreateCollectionIfNotExists(CollectionConstants.ShiftsCollection, db);

                // Create Coverage Requests Collection
                CreateCollectionIfNotExists(CollectionConstants.CoverageRequestsCollection, db);
                // Only one coverage request can exist for a given shift
                var coverageUniqueIndexName=createUniqueIndex(coverage => coverage.ShiftID, db.GetCollection<CoverageRequest>(CollectionConstants.CoverageRequestsCollection));
                LogUniqueCreationSuccess(CollectionConstants.CoverageRequestsCollection, "ShiftID", coverageUniqueIndexName);
                //Create Trade Offers Collection
                CreateCollectionIfNotExists(CollectionConstants.TradeOffersCollection, db);

                //Create Time Off Requests Collection
                CreateCollectionIfNotExists(CollectionConstants.TimeOffRequestsCollection, db);
                //Create Trade Offers Collection
                CreateCollectionIfNotExists(CollectionConstants.PickupOffersCollection, db);
                
                initSucceeded = true;
                _logger.LogInformation($"Successfully initialized {_settings.Database}");
            }
            catch (TimeoutException e)
            {
                _logger.LogWarning("Timeout when attempting to initialize database. Check that server is live. Retrying...");
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Failed to initialize database. Unexpected exception.");
                throw;
            }
            
        }
        
    }
}

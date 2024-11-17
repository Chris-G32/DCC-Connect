using API.Config;
using API.Constants;
using API.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace API.Services;

public interface IDatabaseInitializer
{
    void InitializeDatabase();
}

public class DatabaseInitializer(IConfiguration config, IDBClientProvider clientProvider) : IDatabaseInitializer
{
    private readonly MongoDBSettings _settings = config.GetRequiredSection(DatabaseConstants.DatabaseSettingsSection).Get<MongoDBSettings>() ?? throw new Exception("MongoDB settings not found in appsettings.json");
    private readonly IDBClientProvider _clientProvider = clientProvider;
    private List<string> _collectionNames;
    /// <summary>
    /// This ensures a field is unique between entries in a collection. If a field is not unique, an exception will be thrown when trying to insert a document with a duplicate value.
    /// No validation is needed to be done before calling this method, as it will be ignored if the index already exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fieldSpecifier"></param>
    /// <param name="collection"></param>
    internal void createUniqueIndex<T>(Expression<Func<T, object>> fieldSpecifier, IMongoCollection<T> collection) where T : class
    {
        var indexKeysDefinition = Builders<T>.IndexKeys.Ascending(fieldSpecifier);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<T>(indexKeysDefinition, indexOptions);
        var res=collection.Indexes.CreateOne(indexModel);
    }
    private void CreateCollectionIfNotExists(string collectionName,IMongoDatabase db)
    {
        //Create Employee Collection
        if (!_collectionNames.Contains(collectionName))
        {
            db.CreateCollection(collectionName);
        }
    }
    public void InitializeDatabase()
    {
        var client = _clientProvider.Client;
        var db = client.GetDatabase(DatabaseConstants.DatabaseName);

        _collectionNames = db.ListCollectionNames().ToList();
        //Create Employee Collection
        CreateCollectionIfNotExists(CollectionConstants.EmployeesCollection, db);

        //Create Shift Collection
        CreateCollectionIfNotExists(CollectionConstants.ShiftsCollection, db);

        // Create Coverage Requests Collection
        CreateCollectionIfNotExists(CollectionConstants.CoverageRequestsCollection, db);
        // Only one coverage request can exist for a given shift
        createUniqueIndex(coverage => coverage.ShiftID, db.GetCollection<CoverageRequest>(CollectionConstants.CoverageRequestsCollection));

        //Create Trade Offers Collection
        CreateCollectionIfNotExists(CollectionConstants.TradeOffersCollection, db);

        //Create Time Off Requests Collection
        CreateCollectionIfNotExists(CollectionConstants.TimeOffRequestsCollection, db);
        //Create Trade Offers Collection
        CreateCollectionIfNotExists(CollectionConstants.PickupOffersCollection, db);

        //Create users collection
        CreateCollectionIfNotExists(CollectionConstants.UsersCollection, db);
        createUniqueIndex(user => user.Email, db.GetCollection<User>(CollectionConstants.UsersCollection));
    }
}

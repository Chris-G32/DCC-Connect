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
    public void InitializeDatabase()
    {
        var client = _clientProvider.Client;
        var db = client.GetDatabase(DatabaseConstants.DatabaseName);

        var collectionNames = db.ListCollectionNames().ToList();
        //Create Employee Collection
        if (!collectionNames.Contains(CollectionConstants.EmployeesCollection))
        {
            db.CreateCollection(CollectionConstants.EmployeesCollection);
        }
        // A given email can only be assigned to one employee
        createUniqueIndex(e => e.Email, db.GetCollection<Employee>(CollectionConstants.EmployeesCollection));

        //Create Shift Collection
        if (!collectionNames.Contains(CollectionConstants.ShiftsCollection))
        {
            db.CreateCollection(CollectionConstants.ShiftsCollection);
        }

        if (!collectionNames.Contains(CollectionConstants.AssignedShiftsCollection))
        {
            db.CreateCollection(CollectionConstants.AssignedShiftsCollection);
        }
        // A given shift can only be assigned to one employee
        createUniqueIndex(assignment => assignment.ShiftID, db.GetCollection<ShiftAssignment>(CollectionConstants.AssignedShiftsCollection));

        if (!collectionNames.Contains(CollectionConstants.OfferedUpShifts))
        {
            db.CreateCollection(CollectionConstants.OfferedUpShifts);
        }
        createUniqueIndex(offer => offer.ShiftId, db.GetCollection<ShiftOffer>(CollectionConstants.OfferedUpShifts));

        if (!collectionNames.Contains(CollectionConstants.TradeRequestsCollection))
        {
            db.CreateCollection(CollectionConstants.TradeRequestsCollection);
        }
        createUniqueIndex(req=>req., db.GetCollection<ShiftPickupRequest>(CollectionConstants.TradeRequestsCollection));

    }
}

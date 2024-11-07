using API.Constants;
using API.Errors;
using API.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Runtime.Serialization;

namespace API.Services;

public interface IEntityRetriever
{
    public T GetEntityOrThrow<T>(IMongoCollection<T> collection, ObjectId? ID) where T : MongoObject;
    public bool DoesEntityExist<T>(IMongoCollection<T> collection, ObjectId? ID) where T : MongoObject;
    public void AssertEntityExists<T>(IMongoCollection<T> collection, ObjectId? ID) where T : MongoObject;
    /// <summary>
    /// Exposing this for simplicity of other object constructors. 
    /// Odds are if you need the utility of this, you need the collections provider. Allows for more complicated query logic too
    /// </summary>
    ICollectionsProvider CollectionsProvider { get; }
}

public class EntityRetriever(ICollectionsProvider collectionsProvider) : IEntityRetriever
{
    public ICollectionsProvider CollectionsProvider { get; } = collectionsProvider;

    public T GetEntityOrThrow<T>(IMongoCollection<T> collection, ObjectId? ID) where T : MongoObject
    {
        var res= collection.Find(mongoObject => mongoObject.Id == ID).FirstOrDefault()
            ?? throw new EntityDoesNotExistException(ID.ToString(), collection.CollectionNamespace.CollectionName);
        return res;
    }
    public bool DoesEntityExist<T>(IMongoCollection<T> collection, ObjectId? ID) where T : MongoObject
    {
        var count = collection.CountDocuments(mongoObject => mongoObject.Id == ID);
        return count > 0;
    }

    public void AssertEntityExists<T>(IMongoCollection<T> collection, ObjectId? ID) where T : MongoObject
    {
        var count = collection.CountDocuments(mongoObject => mongoObject.Id == ID);
        if (count == 0)
        {
            throw new EntityDoesNotExistException(ID.ToString(), collection.CollectionNamespace.CollectionName);
        }
    }
}
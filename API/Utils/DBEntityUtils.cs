using API.Models;
using MongoDB.Driver;

namespace API.Utils;

public class DBEntityUtils
{
    /// <summary>
    /// Verifies the ID is found in a collection. Assumes uniqueness of ID's
    /// </summary>
    /// <typeparam name="T">Type of object in collection</typeparam>
    /// <param name="collection">Collection to check for object with id</param>
    /// <param name="ID">ID to find</param>
    /// <returns>True when any object with ID is found </returns>
    static public bool EntityWithIDExists<T>(IMongoCollection<T> collection, string ID) where T : MongoObject
    {
        return collection.CountDocuments(mongoObject => mongoObject.Id.ToString() == ID) > 0;
    }
    /// <summary>
    /// Throws an exception if the ID is not found in the collection. Assumes uniqueness of IDs.
    /// </summary>
    /// <typeparam name="T">Type of object in the collection.</typeparam>
    /// <param name="collection">Collection to check for object with ID.</param>
    /// <param name="ID">ID to find.</param>
    /// <param name="message">Message to include in the exception if the ID is not found.</param>
    /// <exception cref="Exception">Thrown when the ID is not found.</exception>
    static public void ThrowIfNotExists<T>(IMongoCollection<T> collection, string ID, string? message = null) where T : MongoObject
    {
        if (message == null)
        {
            message = ErrorUtils.FormatObjectDoesNotExistErrorString(ID, collection.CollectionNamespace.CollectionName);
        }
        if (!EntityWithIDExists(collection, ID)) { throw new Exception(message); }
    }
    /// <summary>
    /// Throws an exception if the ID is not found in the collection. Assumes uniqueness of IDs.
    /// </summary>
    /// <typeparam name="T">Type of object in the collection.</typeparam>
    /// <param name="collection">Collection to check for object with ID.</param>
    /// <param name="ID">ID to find.</param>
    /// <param name="exception">Exception to throw if not exists.</param>
    /// <exception cref="Exception">Thrown when the ID is not found.</exception>
    static public void ThrowIfNotExists<T>(IMongoCollection<T> collection, string ID, Exception exception) where T : MongoObject
    {
        if (!EntityWithIDExists(collection, ID)) { throw exception; }
    }
}

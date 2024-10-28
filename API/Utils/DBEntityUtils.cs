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
    /// <returns>Found when any object with ID is found or null </returns>
    static public T? TryGetEntityByID<T>(IMongoCollection<T> collection, string? ID) where T : MongoObject
    {
        return collection.Find(mongoObject => mongoObject.Id.ToString() == ID).FirstOrDefault();
    }
    /// <summary>
    /// Throws an exception if the ID is not found in the collection. Assumes uniqueness of IDs.
    /// </summary>
    /// <typeparam name="T">Type of object in the collection.</typeparam>
    /// <param name="collection">Collection to check for object with ID.</param>
    /// <param name="ID">ID to find.</param>
    /// <param name="message">Message to include in the exception if the ID is not found.</param>
    /// <returns> The item found with the id</returns>
    /// <exception cref="Exception">Thrown when the ID is not found.</exception>
    static public T ThrowIfNotExists<T>(IMongoCollection<T> collection, string? ID, string? message = null) where T : MongoObject
    {
        if (message == null)
        {
            message = ErrorUtils.FormatObjectDoesNotExistErrorString(ID, collection.CollectionNamespace.CollectionName);
        }
        var result = TryGetEntityByID(collection, ID);
        if (result == null)
        {
            throw new Exception(message);
        }
        return result;
    }
    /// <summary>
    /// Throws an exception if the ID is not found in the collection. Assumes uniqueness of IDs.
    /// </summary>
    /// <typeparam name="T">Type of object in the collection.</typeparam>
    /// <param name="collection">Collection to check for object with ID.</param>
    /// <param name="ID">ID to find.</param>
    /// <param name="exception">Exception to throw if not exists.</param>
    /// <returns> The item found with the id</returns>
    /// <exception cref="Exception">Thrown when the ID is not found.</exception>
    static public T ThrowIfNotExists<T>(IMongoCollection<T> collection, string? ID, Exception exception) where T : MongoObject
    {
        var result = TryGetEntityByID(collection, ID);
        if (result == null)
        {
            throw exception;
        }
        return result;
    }
}

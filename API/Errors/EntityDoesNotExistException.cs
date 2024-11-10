using MongoDB.Driver;

namespace API.Errors;

public class EntityDoesNotExistException : DCCApiException
{
    public EntityDoesNotExistException(string? objectId, string collection) : base($"Failed to find object with ID {objectId} in collection {collection}.")
    {
    }
}
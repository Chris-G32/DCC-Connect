using MongoDB.Bson;

namespace API.Models.QueryOptions;

public interface IHaveIdFilterOption
{
    /// <summary>
    /// Id of the object in database. _id in MongoDB.
    /// </summary>
    ObjectId? UniqueID { get; set; }
}

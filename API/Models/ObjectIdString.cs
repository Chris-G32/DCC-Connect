using MongoDB.Bson;

namespace API.Models;
public class ObjectIdString
{
    private ObjectId _id;
    public ObjectId id
    {
        get { return _id; }
        set { _id = value; }
    }
}
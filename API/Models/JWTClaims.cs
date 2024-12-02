using MongoDB.Bson;

namespace API.Models;

public struct JWTClaims
{
    public required ObjectId UserID { get; set; }
    public required string Role { get; set; }
}

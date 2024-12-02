using API.Constants;
using API.Models;
using MongoDB.Bson;
using System.Security.Claims;

namespace API.Utils;

public class AuthUtils
{
    public static JWTClaims GetClaims(HttpRequest request)
    {
        var userId = request.HttpContext.User.FindFirst("userId")?.Value ?? throw new Exception("Failed to get userId from claim");
        var role= request.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? throw new Exception("Failed to get role from claim");
        return new JWTClaims
        {
            UserID = ObjectId.Parse(userId),
            Role = role
        };
    }
    public static bool IsAdmin(JWTClaims claims) => claims.Role == RoleConstants.Admin;
}

using API.Constants;
using API.Models.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    string GenerateToken(string userId, string role);
}
public class JwtTokenGenerator: IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GenerateToken(User user)
    {
        var userId=user.Id ?? throw new ArgumentNullException("User ID is null");
        return GenerateToken(userId.ToString(), user.EmployeeRole);
    }
    public string GenerateToken(string userId, string role)
    {
        if(!RoleConstants.ValidRoles.Contains(role))
        {
            throw new ArgumentException("Invalid role provided for JWT.");
        }
        var claims = new[]
        {
            new Claim("userId", userId),
            new Claim("role", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var jwtSecret = _configuration["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Secret does not exist, please specify in secrets.json");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),//Arbitrary expiration time, figure 2 hours is fair 
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

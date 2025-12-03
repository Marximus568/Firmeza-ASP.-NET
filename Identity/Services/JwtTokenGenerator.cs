using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdminDashboard.Identity.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AdminDashboard.Identity.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUserIdentity user, IEnumerable<string> roles);
}

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(ApplicationUserIdentity user, IEnumerable<string> roles)
    {

        var keyString = _configuration["Jwt__Key"] 
                        ?? Environment.GetEnvironmentVariable("Jwt__Key");

        var issuer = _configuration["Jwt__Issuer"];
        var audience = _configuration["Jwt__Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));

        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

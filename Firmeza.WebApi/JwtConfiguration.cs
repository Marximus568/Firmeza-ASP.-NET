using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Firmeza.WebApi;

public static class JwtConfiguration
{
    // Extension method to configure JWT Authentication
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Retrieve secret key from environment or appsettings.json
        var key = configuration["Jwt:Key"]
                  ?? Environment.GetEnvironmentVariable("Jwt__Key")
                  ?? throw new Exception("JWT Key not found in configuration or environment variables.");

        // Convert key string into byte array for cryptographic operations
        var encodedKey = Encoding.UTF8.GetBytes(key);

        // Register authentication services
        services.AddAuthentication(options =>
            {
                // Default scheme for authentication and challenges
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Token validation configuration
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false, // Skips issuer validation (you can enable it later)
                    ValidateAudience = false, // Skips audience validation
                    ValidateLifetime = true, // Ensures token expiration is respected
                    ValidateIssuerSigningKey = true, // Validates the token signature
                    IssuerSigningKey = new SymmetricSecurityKey(encodedKey) // Key used to verify JWT signature
                };
            });

        return services;
    }
}


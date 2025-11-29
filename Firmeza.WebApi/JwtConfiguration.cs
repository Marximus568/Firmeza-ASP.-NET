using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Firmeza.WebApi;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Read from configuration and environment (Docker)
        var key = configuration["Jwt__Key"]
                  ?? Environment.GetEnvironmentVariable("Jwt__Key")
                  ?? throw new Exception("JWT Key not found.");

        var issuer = configuration["Jwt__Issuer"];
        var audience = configuration["Jwt__Audience"];

        var encodedKey = Encoding.UTF8.GetBytes(key);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Enable issuer and audience (needed because your generator uses them)
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(encodedKey)
                };
            });

        return services;
    }
}

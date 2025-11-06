using AdminDashboard.Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AdminDashboard.Identity.Design;

/// <summary>
/// Design-time factory so 'dotnet ef' can create migrations for AdminDashboard.Identity
/// </summary>
public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<IdentityContext>();

        // Read connection string from environment variable DB_CONNECTION
        var conn = Environment.GetEnvironmentVariable("DB_CONNECTION");
        if (string.IsNullOrWhiteSpace(conn))
        {
            // Default to localhost postgres if not set (don't overwrite in production)
            conn = "Host=localhost;Port=5432;Database=FirmezaDatabase;Username=postgres;Password=postgres";
        }

        builder.UseNpgsql(conn, b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));

        return new IdentityContext(builder.Options);
    }
}


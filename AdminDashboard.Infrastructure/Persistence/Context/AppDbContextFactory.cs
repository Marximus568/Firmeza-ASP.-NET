using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System.Reflection;

namespace AdminDashboard.Infrastructure.Persistence.Context;

/// <summary>
/// Design-time factory for EF Core migrations.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // -------------------------------------------------
        // 1. Load .env (if you use one) – DotNetEnv package
        // -------------------------------------------------
        try
        {
            var envPath = Path.Combine(GetProjectRoot(), ".env");
            if (File.Exists(envPath))
                DotNetEnv.Env.Load(envPath);
        }
        catch { /* ignore */ }

        // -------------------------------------------------
        // 2. Build configuration (appsettings + env vars)
        // -------------------------------------------------
        var config = new ConfigurationBuilder()
            .SetBasePath(GetProjectRoot())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                         optional: true)
            .AddEnvironmentVariables()
            .Build();

        // -------------------------------------------------
        // 3. Connection string – priority order
        // -------------------------------------------------
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
                            ?? config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string not found. " +
                "Set DB_CONNECTION environment variable **or** add 'DefaultConnection' in appsettings.json.");

        // -------------------------------------------------
        // 4. Build DbContextOptions
        // -------------------------------------------------
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure();
            })
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    /// Returns the *project* folder (not bin/Debug/...).
    /// </summary>
    private static string GetProjectRoot()
    {
        var location = Assembly.GetExecutingAssembly().Location;
        var dir      = Path.GetDirectoryName(location)!;   // bin/Debug/net8.0
        return Directory.GetParent(dir)!.FullName;         // project root
    }
}
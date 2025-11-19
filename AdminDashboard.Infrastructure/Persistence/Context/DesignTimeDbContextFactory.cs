
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AdminDashboard.Infrastructure.Persistence.Context
{
    // Factory used by EF Core tools to create the DbContext at design time
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Base path points to the Web project (where appsettings.json is located)
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AdminDashboard.Web");

            // Build configuration from appsettings and environment variables
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Get connection string from environment or fallback to appsettings
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
                                   ?? configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No database connection found. Ensure DB_CONNECTION exists in .env or appsettings.json");

            // Configure DbContext with Npgsql
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
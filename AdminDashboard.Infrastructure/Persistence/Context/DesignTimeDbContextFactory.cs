using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using DotNetEnv;

namespace AdminDashboard.Infrastructure.Persistence.Context
{
    // Factory used by EF Core tools to create the DbContext at design time
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // -------------------------------------
            // 1. Load .env manually (EF does not do it)
            // -------------------------------------
            var solutionRoot = Path.Combine(Directory.GetCurrentDirectory(), "..", "..");
            var envPath = Path.Combine(solutionRoot, ".env");

            if (File.Exists(envPath))
            {
                Env.Load(envPath); // Load variables into Environment
                Console.WriteLine($"Loaded .env from: {envPath}");
            }
            else
            {
                Console.WriteLine("WARNING: .env NOT found at: " + envPath);
            }

            // -------------------------------------
            // 2. Locate appsettings.json (Web project)
            // -------------------------------------
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AdminDashboard.Web");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // Now includes .env vars
                .Build();

            // -------------------------------------
            // 3. Fetch DB_CONNECTION (env takes priority)
            // -------------------------------------
            var connectionString =
                Environment.GetEnvironmentVariable("DB_CONNECTION") ??
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "No database connection found. Ensure DB_CONNECTION exists in `.env` or appsettings.json."
                );

            // -------------------------------------
            // 4. Build DbContext options
            // -------------------------------------
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

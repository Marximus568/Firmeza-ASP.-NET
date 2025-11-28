using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AdminDashboard.Infrastructure.Persistence;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // Migrate AppDbContext (business tables)
            var appContext = serviceProvider.GetRequiredService<AppDbContext>();
            
            logger.LogInformation("[Migration] Checking for pending AppDbContext migrations...");
            
            if ((await appContext.Database.GetPendingMigrationsAsync()).Any())
            {
                logger.LogInformation("[Migration] Applying pending AppDbContext migrations...");
                await appContext.Database.MigrateAsync();
                logger.LogInformation("[Migration] AppDbContext migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("[Migration] No pending AppDbContext migrations found.");
            }

            // Migrate IdentityContext (Identity tables)
            var identityContext = serviceProvider.GetRequiredService<IdentityContext>();
            
            logger.LogInformation("[Migration] Checking for pending IdentityContext migrations...");
            
            if ((await identityContext.Database.GetPendingMigrationsAsync()).Any())
            {
                logger.LogInformation("[Migration] Applying pending IdentityContext migrations...");
                await identityContext.Database.MigrateAsync();
                logger.LogInformation("[Migration] IdentityContext migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("[Migration] No pending IdentityContext migrations found.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Migration] An error occurred while applying migrations.");
            throw;
        }
    }
}

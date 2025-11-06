using AdminDashboard.Identity.Seeders;

namespace AdminDashboard.Infrastructure.Seeders;

/// <summary>
/// Compatibility wrapper: delegates seeding to AdminDashboard.Identity seeder implementation.
/// This keeps call sites (if any) using AdminDashboard.Infrastructure.Seeders.IdentitySeeder working
/// while the canonical seeder lives in AdminDashboard.Identity.
/// </summary>
public static class IdentitySeeder
{
    public static Task SeedAsync(IServiceProvider serviceProvider)
    {
        return AdminDashboard.Identity.Seeders.IdentitySeeder.SeedAsync(serviceProvider);
    }
}

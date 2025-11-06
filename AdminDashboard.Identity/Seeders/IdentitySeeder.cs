using AdminDashboard.Domain.ValueObjects;
using AdminDashboard.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AdminDashboard.Identity.Seeders;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUserIdentity>>();

        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
        await SeedClientUserAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in UserRole.AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUserIdentity> userManager)
    {
        const string adminEmail = "admin@admindashboard.com";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUserIdentity
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, UserRole.Admin);
        }
        else if (!await userManager.IsInRoleAsync(adminUser, UserRole.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, UserRole.Admin);
        }
    }

    private static async Task SeedClientUserAsync(UserManager<ApplicationUserIdentity> userManager)
    {
        const string clientEmail = "client@admindashboard.com";
        const string clientPassword = "Client123!";

        var clientUser = await userManager.FindByEmailAsync(clientEmail);

        if (clientUser == null)
        {
            clientUser = new ApplicationUserIdentity
            {
                UserName = clientEmail,
                Email = clientEmail,
                FirstName = "Test",
                LastName = "Client",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(clientUser, clientPassword);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(clientUser, UserRole.Client);
        }
        else if (!await userManager.IsInRoleAsync(clientUser, UserRole.Client))
        {
            await userManager.AddToRoleAsync(clientUser, UserRole.Client);
        }
    }
}

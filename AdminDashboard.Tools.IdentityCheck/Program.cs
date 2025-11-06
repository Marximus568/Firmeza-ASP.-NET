using AdminDashboard.Identity.DependencyInjection;
using AdminDashboardApplication.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("IdentityCheck: starting...");

// Load .env
EnvLoader.Load();

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(config);

// Add minimal hosting services that Identity expects
services.AddLogging();
services.AddOptions();

// Register identity infra
services.AddIdentityInfrastructure(config);

using var provider = services.BuildServiceProvider();

Console.WriteLine("IdentityCheck: ServiceProvider built");

// Try to run seeder
try
{
    using var scope = provider.CreateScope();
    var sp = scope.ServiceProvider;
    Console.WriteLine("IdentityCheck: calling seeder...");
    await AdminDashboard.Identity.Seeders.IdentitySeeder.SeedAsync(sp);
    Console.WriteLine("IdentityCheck: seeder finished.");

    // Try login with seeded admin user
    var userManager = sp.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<AdminDashboard.Identity.Entities.ApplicationUserIdentity>>();
    var targetEmail = "admin@admindashboard.com";
    var targetPassword = "Admin123!";
    var user = await userManager.FindByEmailAsync(targetEmail);
    if (user == null)
    {
        Console.WriteLine($"IdentityCheck: user {targetEmail} not found");
    }
    else
    {
        var ok = await userManager.CheckPasswordAsync(user, targetPassword);
        Console.WriteLine($"IdentityCheck: password check for {targetEmail} => {ok}");
    }
}
catch (Exception ex)
{
    Console.WriteLine("IdentityCheck: seeder error: " + ex);
}

Console.WriteLine("IdentityCheck: done.");

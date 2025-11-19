using AdminDashboard.Infrastructure;
using AdminDashboard.Infrastructure.Persistence;
using AdminDashboard.Infrastructure.Routing;
using AdminDashboard.Infrastructure.Seeders;
using AdminDashboardApplication;
using AdminDashboardApplication.Common;
using DotNetEnv;

Env.Load("../.env");

var builder = WebApplication.CreateBuilder(args);

// ===================================================
// 🔧 SERVICE REGISTRATION
// ===================================================

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 1️⃣ Persistence (DbContext)
builder.Services.AddPersistence(builder.Configuration);

// 2️⃣ Infrastructure (Identity, JWT, Repositories, Domain Services)
builder.Services.AddInfrastructure(builder.Configuration);

// 3️⃣ Application Layer (UseCases + Handlers + AutoMapper)
builder.Services.AddApplication();

// 4️⃣ Custom App Routing
builder.Services.AddAppRouting();

// 5️⃣ Identity cookie configuration (UI behavior)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

var app = builder.Build();

// ===================================================
// 🧪 SEEDERS (Optional)
// ===================================================
if (Environment.GetEnvironmentVariable("RUN_SEEDERS") == "1")
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    Console.WriteLine("[Seeder] Starting Identity seeder...");

    try
    {
        await AdminDashboard.Identity.Seeders.IdentitySeeder.SeedAsync(services);
        Console.WriteLine("[Seeder] Identity seeder finished.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Seeder] ERROR: {ex}");
    }
}

// ===================================================
// 🌐 MIDDLEWARE PIPELINE
// ===================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    await next();
});

app.UseRouting();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Routes
app.UseAppRouting();

// Start
app.Run();

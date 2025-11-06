using AdminDashboard.Infrastructure;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Infrastructure.Routing;
using AdminDashboard.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using AdminDashboardApplication.Common;

// Load solution-level .env (if present) so configuration picks up DB_CONNECTION and other vars
EnvLoader.Load();

// Debug: print whether DB_CONNECTION is set (do not print full value in logs to avoid secrets)
var _dbConn = Environment.GetEnvironmentVariable("DB_CONNECTION");
if (!string.IsNullOrEmpty(_dbConn))
{
    Console.WriteLine($"[EnvLoader] DB_CONNECTION found, length={_dbConn.Length}");
}
else
{
    Console.WriteLine("[EnvLoader] DB_CONNECTION NOT FOUND");
}

var builder = WebApplication.CreateBuilder(args);

// ===================================================
// 🔧 SERVICES
// ===================================================

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Infrastructure (DbContext, Identity, Cookies)
builder.Services.AddInfrastructure(builder.Configuration);

// Routing & authorization
builder.Services.AddAppRouting();

// Configure cookie paths for Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

var app = builder.Build();

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

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ===================================================
// 🚀 ROUTING
// ===================================================
app.UseAppRouting();

// ===================================================
// ▶️ RUN APP
// ===================================================
app.Run();

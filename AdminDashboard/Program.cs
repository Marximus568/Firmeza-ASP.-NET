using AdminDashboard.Application.DTOs.User.Interfaces;
using AdminDashboard.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ===================================================
// ADD SERVICES TO CONTAINER
// ===================================================
builder.Services.AddRazorPages();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ===================================================
// ADD INFRASTRUCTURE SERVICES
// Includes:
// - Loading .env via _EnvLoader
// - Configuring DbContexts
// - Setting up Identity & Cookies
// - Registering Domain Services
// ===================================================
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ===================================================
// CONFIGURE HTTP REQUEST PIPELINE
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
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Health endpoint
app.MapGet("/health", async (AppDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    return Results.Ok(new { status = canConnect ? "Healthy" : "Unreachable" });
});

app.MapRazorPages();
app.Run();
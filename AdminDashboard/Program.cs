using AdminDashboard.Application.DTOs.User.Interfaces;
using AdminDashboard.Infrastructure;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Infrastructure.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===================================================
// ADD SERVICES TO CONTAINER
// ===================================================

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Infrastructure (DbContext, Identity, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Routing & Authorization policies (AdminOnly, etc.)
builder.Services.AddAppRouting();

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

// Custom routing from your Routing folder
app.MapAppRoutes();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Health check endpoint (optional, can move to routing)
app.MapGet("/health", async (AppDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    return Results.Ok(new { status = canConnect ? "Healthy" : "Unreachable" });
});

app.Run();
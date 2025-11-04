using AdminDashboard.Infrastructure;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Infrastructure.Routing;
using Microsoft.EntityFrameworkCore;

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
// 🗄️ DATABASE MIGRATIONS
// ===================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// ===================================================
// ▶️ RUN APP
// ===================================================
app.Run();

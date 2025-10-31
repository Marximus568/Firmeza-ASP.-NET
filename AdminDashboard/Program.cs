using AdminDashboard.Infrastructure;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// ===================================================
// ADD SERVICES TO CONTAINER
// ===================================================
builder.Services.AddRazorPages();

// ===================================================
// ADD INFRASTRUCTURE SERVICES
// This includes:
// - Loading .env via _EnvLoader
// - Configuring DbContexts (AppDbContext & IdentityContext)
// - Setting up Identity & Cookie Authentication
// - Registering Domain Services and UseCases
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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
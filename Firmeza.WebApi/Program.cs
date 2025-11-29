using System.Reflection;
using AdminDashboard.Infrastructure;
using AdminDashboard.Infrastructure.Filters;
using AdminDashboardApplication;
using DotNetEnv;
using Firmeza.WebApi;
using Microsoft.OpenApi.Models;
using AdminDashboard.Identity.Persistence.Context;
using AdminDashboard.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// ====================================
// Load environment variables
// ====================================
Env.Load("../.env");

// ====================================
// Authentication (JWT)
// ====================================
builder.Services.AddJwtAuthentication(builder.Configuration);

// ====================================
// Application Layer (Use Cases)
// ====================================
builder.Services.AddApplication();

// ====================================
// Infrastructure (DbContext, Repositories, Services)
// ====================================
builder.Services.AddInfrastructure(builder.Configuration);

// ====================================
// Authorization
// ====================================
builder.Services.AddAuthorization();


// ====================================
// MVC Controllers + Global Filters
// ====================================

builder.Services.AddControllers(options =>
{
    // Global filter to protect sensitive info
    options.Filters.Add<SensitiveDataFilter>();
});

// ====================================
// CORS Policy (Allow React frontend)
// ====================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174", 
                "http://localhost:5175",
                "http://localhost:5176",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ====================================
// Swagger + JWT Support
// ====================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Firmeza API",
        Version = "v1"
    });

    // XML Comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    // JWT Auth in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Add 'Bearer {token}' to authenticate",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ====================================
// Build App
// ====================================
var app = builder.Build();

// ====================================
// Middleware Pipeline
// ====================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// ====================================
// CORS must be before Authentication/Authorization
// ====================================
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ====================================
// Apply Migrations (if configured)
// ====================================
if (Environment.GetEnvironmentVariable("RUN_MIGRATIONS") == "true")
{
    await app.Services.ApplyMigrationsAsync();
    
    if (Environment.GetEnvironmentVariable("ONLY_MIGRATE") == "true")
    {
        return;
    }
}

// ====================================
// Run Seeders (if configured)
// ====================================
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

app.Run();

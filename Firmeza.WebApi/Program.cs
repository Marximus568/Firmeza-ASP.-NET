using System.Reflection;
using AdminDashboard.Infrastructure;
using AdminDashboard.Infrastructure.Filters;
using DotNetEnv;
using Firmeza.WebApi;
using Microsoft.OpenApi.Models;


// ====================================
// 🚀 Create the WebApplication builder
// ====================================
var builder = WebApplication.CreateBuilder(args);

// ====================================
// 📦 Load environment variables (.env)
// ====================================
Env.Load("../.env");

// ====================================
// 🔐 Register Authentication + Infrastructure Services
// ====================================
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// ====================================
// 🛡️ Authorization (Required for JWT + [Authorize])
// ====================================
builder.Services.AddAuthorization();

// ====================================
// 📌 Controllers + Global Filters
// ====================================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers(options =>
{
    // Global filter to avoid returning sensitive data
    options.Filters.Add<SensitiveDataFilter>();
});

// ====================================
// 📄 Swagger / OpenAPI Documentation
// ====================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // Basic API info
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Firmeza API",
        Version = "v1",
        Description = "Backend API for Firmeza platform"
    });

    // Enable XML comments (if enabled in csproj)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Enable Swagger annotations
    c.EnableAnnotations();

    // ================================
    // 🔐 Configure JWT Bearer in Swagger
    // ================================
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer eyJhbGciOi...'",
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
// 🚀 Build the WebApplication
// ====================================
var app = builder.Build();

// ====================================
// 🧱 Middlewares (order matters)
// ====================================

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Authentication middleware
app.UseAuthentication();

// Authorization middleware
app.UseAuthorization();

// Map controllers to endpoints
app.MapControllers();

// Run the application
app.Run();

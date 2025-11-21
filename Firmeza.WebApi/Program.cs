using System.Reflection;
using AdminDashboard.Infrastructure;
using AdminDashboard.Infrastructure.Filters;
using AdminDashboardApplication;
using DotNetEnv;
using Firmeza.WebApi;
using Microsoft.OpenApi.Models;
using AdminDashboard.Identity.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityContext>();

// ====================================
// 📦 Load environment variables
// ====================================
Env.Load("../.env");

// ====================================
// 🔐 Authentication (JWT)
// ====================================
builder.Services.AddJwtAuthentication(builder.Configuration);

// ====================================
// 🧱 Application Layer (Use Cases)
// ====================================
builder.Services.AddApplication();

// ====================================
// 🏗️ Infrastructure (DbContext, Repositories, Services)
// ====================================
builder.Services.AddInfrastructure(builder.Configuration);

// ====================================
// 📧 SMTP (Email service)
// ====================================
builder.Services.Configure<SmtpSettings.SmtpSettings>(options =>
{
    options.Host = Environment.GetEnvironmentVariable("SMTP_HOST");
    options.Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")!);
    options.From = Environment.GetEnvironmentVariable("SMTP_FROM");
    options.FromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME");
    options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
    options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
    options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL") ?? "true");
});

// ====================================
// 🔐 Authorization
// ====================================
builder.Services.AddAuthorization();

// ====================================
// 📌 Controllers + Global Filters
// ====================================
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers(options =>
{
    // Global filter to protect sensitive info
    options.Filters.Add<SensitiveDataFilter>();
});

// ====================================
// 📄 Swagger + JWT Support
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
// 🚀 Build App
// ====================================
var app = builder.Build();

// ====================================
// 🧱 Middleware Pipeline
// ====================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

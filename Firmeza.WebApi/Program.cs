using AdminDashboard.Infrastructure;
using AdminDashboard.Infrastructure.Filters;
using DotNetEnv;
using Firmeza.WebApi;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ====================================
// 📦 Load environment variables from .env
// ====================================
Env.Load("../.env"); // Adjust path if .env is in the parent directory

// ====================================
// 🔐 Add JWT authentication from config
// ====================================
builder.Services.AddJwtAuthentication(builder.Configuration);

// Infrastructure (DbContext, Identity, Cookies)
builder.Services.AddInfrastructure(builder.Configuration);
// ==============================
// 🧾 Add Authorization (required for [Authorize])
// ==============================
builder.Services.AddAuthorization();
//Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<SensitiveDataFilter>();
});
// ==============================
// 🔧 Other services (Swagger etc.)
// ==============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Firmeza API",
        Version = "v1",
        Description = "API for Firmeza backend services"
    });

    c.EnableAnnotations();
});

var app = builder.Build();

// ==============================
// 🚦 Middlewares (order matters)
// ==============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Validate JWT tokens
app.UseAuthorization();  // Enforce [Authorize] and policies

app.MapControllers();




app.Run();


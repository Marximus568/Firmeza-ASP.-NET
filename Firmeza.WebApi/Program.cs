using AdminDashboard.Infrastructure;
using DotNetEnv;
using Firmeza.WebApi;

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
builder.Services.AddControllers();

// ==============================
// 🔧 Other services (Swagger etc.)
// ==============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


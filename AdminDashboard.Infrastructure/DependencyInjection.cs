using AdminDashboard.Contracts.Users;
using AdminDashboard.Application.Interfaces;
using AdminDashboard.Application.UseCases.Auth;
using AdminDashboard.Contracts;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Infrastructure.Services;
using AdminDashboardApplication.Common;
using AdminDashboardApplication.DTOs.Auth.Interfaces;
using AdminDashboardApplication.DTOs.Auth.UseCases;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AdminDashboard.Identity.DependencyInjection;
using AdminDashboard.Infrastructure.Services.ProductServices;
using AdminDashboard.Infrastructure.Services.UsersServices;
using AdminDashboardApplication.DTOs.Products.Interfaces;
using AdminDashboardApplication.DTOs.Users.Interface;

namespace AdminDashboard.Infrastructure;

/// <summary>
/// Configures all Infrastructure services for Dependency Injection.
/// Keeps Program.cs clean and isolates implementation details.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===================================================
        // LOAD .env USING UTIL (moved to Identity project where needed)
        // ===================================================
        // EnvLoader.Load();  // removed to avoid duplicate loading and responsibility

        // ===================================================
        // GET DATABASE CONNECTION
        // ===================================================
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Missing .env or DB_CONNECTION variable");
        }
      

        // ============================
        // DATABASE CONTEXT (Domain Data)
        // ============================
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        // ============================
        // IDENTITY INFRASTRUCTURE
        // ============================
        // Identity setup moved to AdminDashboard.Identity project. Use its extension here.
        services.AddIdentityInfrastructure(configuration);

        // ============================
        // COOKIE AUTHENTICATION
        // ============================
        // Move cookie configuration up here if still required by Infrastructure
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.SlidingExpiration = true;
            options.ReturnUrlParameter = "returnUrl";
        });

        // ============================
        // DOMAIN SERVICES (Ports/Adapters)
        // ============================
        // Identity service implementations are registered inside AdminDashboard.Identity
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IProductServices, ProductService>();

        // ============================
        // USE CASES
        // ============================
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<LoginUserUseCase>();
        services.AddScoped<AssignRoleUseCase>();
        

        return services;
    }
}
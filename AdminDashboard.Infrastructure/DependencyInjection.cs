using AdminDashboard.Contracts.Users;
using AdminDashboard.Application.Interfaces;
using AdminDashboard.Application.UseCases.Auth;
using AdminDashboard.Infrastructure.Identity.Entities;
using AdminDashboard.Infrastructure.Identity.Services;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Infrastructure.Services;
using AdminDashboardApplication.Common;
using AdminDashboardApplication.DTOs.Auth.Interfaces;
using AdminDashboardApplication.DTOs.Auth.UseCases;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        // LOAD .env USING UTIL
        // ===================================================
        EnvLoader.Load();

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
        // IDENTITY CONTEXT (Auth Data)
        // ============================
        services.AddDbContext<IdentityContext>(options =>
            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)
            )
        );

        // ============================
        // ASP.NET CORE IDENTITY CONFIGURATION
        // ============================
        services.AddIdentity<ApplicationUserIdentity, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        // ============================
        // COOKIE AUTHENTICATION
        // ============================
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
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUsersService, UsersService>();

        // ============================
        // USE CASES
        // ============================
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<LoginUserUseCase>();
        services.AddScoped<AssignRoleUseCase>();
        

        return services;
    }
}
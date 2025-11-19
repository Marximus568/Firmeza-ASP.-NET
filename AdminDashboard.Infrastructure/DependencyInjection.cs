using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.Interfaces.Repository;
using AdminDashboard.Infrastructure.Repositories.CustomerRepository;
using AdminDashboard.Infrastructure.Repositories.ProductRepository;
using AdminDashboard.Infrastructure.Services.ProductServices;
using AdminDashboard.Infrastructure.Services.UsersServices;
using AdminDashboardApplication.Interfaces;
using AdminDashboardApplication.DTOs.Users.Interface;
using AdminDashboardApplication.DTOs.Products.Interfaces;
using AdminDashboard.Infrastructure.Email;
using AdminDashboard.Identity.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdminDashboard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ======================================
        // 🌱 Environment + Database
        // ======================================
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Missing .env or DB_CONNECTION variable");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        // ======================================
        // 🔐 Identity + JWT
        // ======================================
        services.AddIdentityInfrastructure(configuration);

        // ======================================
        // 📦 Domain Services
        // ======================================
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IProductServices, ProductService>();

        // ======================================
        // 🗄️ Repositories
        // ======================================
        services.AddScoped<IProductRepository, ProductsRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // ======================================
        // ✉️ Email
        // ======================================
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}

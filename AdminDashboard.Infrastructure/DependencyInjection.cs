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
using AdminDashboardApplication.Services;
using AdminDashboard.Infrastructure.Services.PasswordHasher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdminDashboard.Infrastructure;

// Helper to initial infrastructure
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ======================================
        // Environment + Database
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
        // Automapper
        // ======================================
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // ======================================
        // Identity + JWT
        // ======================================
        services.AddIdentityInfrastructure(configuration);

        // ======================================
        // Domain Services
        // ======================================
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IProductServices, ProductService>();

        // ======================================
        // Repositories
        // ======================================
        services.AddScoped<IProductRepository, ProductsRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // ======================================
        // Email
        // ======================================
        services.Configure<AdminDashboard.Infrastructure.SmtpSettings.SmtpSettings>(options =>
        {
            options.Host = configuration["SMTP_HOST"] ?? "";
            options.Port = int.Parse(configuration["SMTP_PORT"] ?? "587");
            options.From = configuration["SMTP_FROM"] ?? "";
            options.FromName = configuration["SMTP_FROM_NAME"] ?? "";
            options.Username = configuration["SMTP_USERNAME"] ?? "";
            options.Password = configuration["SMTP_PASSWORD"] ?? "";
            options.EnableSsl = bool.Parse(configuration["SMTP_ENABLE_SSL"] ?? "true");
            options.UseStartTls = bool.Parse(configuration["SMTP_USE_STARTTLS"] ?? "false");
        });
        
        // Register Email Service
        services.AddScoped<AdminDashboard.Infrastructure.SmtpSettings.SmtpEmailService>();
        services.AddScoped<IEmailService, SmtpEmailServiceAdapter>();
        
        // Register Password Hasher Service
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // ======================================
        // PDF Services
        // ======================================
        services.AddScoped<AdminDashboard.Application.Interfaces.SalePDF.IPdfReportService, AdminDashboard.Infrastructure.SalePDF.Services.PdfReportService>();
        services.AddScoped<AdminDashboard.Application.Interfaces.SalePDF.IPdfService, AdminDashboard.Infrastructure.SalePDF.Services.PdfService>();

        // ======================================
        // Excel Services
        // ======================================
        services.AddScoped<AdminDashboard.Application.Interfaces.ExcelImporter.IExcelImporter, AdminDashboard.Infrastructure.ExcelImporter.Services.ExcelImporterServices>();
        // services.AddScoped<IExcelExporter, ExcelExporterServices>(); // Uncomment if interface exists

        return services;
    }
}

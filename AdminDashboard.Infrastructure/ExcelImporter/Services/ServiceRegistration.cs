using AdminDashboard.Application.Interfaces.ExcelImporter;
using Microsoft.Extensions.DependencyInjection;

namespace AdminDashboard.Infrastructure.ExcelImporter.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddExcelServices(this IServiceCollection services)
    {
        services.AddScoped<IExcelImporter, ExcelImporterServices>();
        services.AddScoped<IExcelExporter, ExcelExporterServices>();
        return services;
    }
}
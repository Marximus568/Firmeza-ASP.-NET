using ExcelImporter.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ExcelImporter.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddExcelServices(this IServiceCollection services)
    {
        services.AddScoped<IExcelImporter, ExcelImporterServices>();
        services.AddScoped<IExcelExporter, ExcelExporterServices>();
        return services;
    }
}
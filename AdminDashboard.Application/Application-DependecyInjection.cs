using AdminDashboard.Application.UseCases.Auth;
using AdminDashboardApplication.Auth.UseCases;
using AdminDashboardApplication.UseCases.Customers;
using Microsoft.Extensions.DependencyInjection;

namespace AdminDashboardApplication;

public static class ApplicationDependecyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AUTH
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<LoginUserUseCase>();
        services.AddScoped<AssignRoleUseCase>();

        // CUSTOMERS
        services.AddScoped<RegisterCustomerHandler>();



        // AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}
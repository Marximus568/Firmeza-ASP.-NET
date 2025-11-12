using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using AdminDashboardApplication.Profile;

namespace AdminDashboard.Infrastructure.Configuration
{
    public static class AutoMapperServiceExtensions
    {
        // Register all AutoMapper profiles located in the same assembly as SaleProfile
        public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(SaleProfile).Assembly);
            return services;
        }
    }
}
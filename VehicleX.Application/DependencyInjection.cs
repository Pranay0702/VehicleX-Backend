using Microsoft.Extensions.DependencyInjection;
using VehicleX.Application.Interfaces;
using VehicleX.Application.Services;

namespace VehicleX.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISalesManagementService, SalesManagementService>();
        return services;
    }
}

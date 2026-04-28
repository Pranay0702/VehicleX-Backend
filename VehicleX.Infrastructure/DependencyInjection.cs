using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleX.Application.Interfaces;
using VehicleX.Infrastructure.Data;
using VehicleX.Infrastructure.Repositories;

namespace VehicleX.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<ISalesInvoiceRepository, SalesInvoiceRepository>();
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        return services;
    }
}

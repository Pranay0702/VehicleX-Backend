using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleX.Application.Interfaces;
using VehicleX.Infrastructure.Data;
using VehicleX.Infrastructure.Repositories;
using VehicleX.Infrastructure.Security;

namespace VehicleX.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'DefaultConnection' is missing.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}

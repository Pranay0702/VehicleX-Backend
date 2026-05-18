using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Infrastructure.Email;
using VehicleX.Infrastructure.Data;
using VehicleX.Infrastructure.Services;
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
        
        services.AddScoped<IVendorRepository,              VendorRepository>();
        services.AddScoped<IPartRepository,                PartRepository>();
        services.AddScoped<ICustomerRepository,            CustomerRepository>();
        services.AddScoped<ISalesInvoiceRepository,        SalesInvoiceRepository>();
        services.AddScoped<IPurchaseInvoiceRepository,     PurchaseInvoiceRepository>();
        services.AddScoped<IPurchaseRepository,            PurchaseRepository>();
        services.AddScoped<IStaffRepository,               StaffRepository>();
        services.AddScoped<IRepositoryManager,             RepositoryManager>();
        services.AddScoped<IJwtTokenService,               JwtTokenService>();
        services.AddScoped<IEmailService,                  EmailService>();
        // services.Configure<EmailSettings>(...);
        return services;
    }
}

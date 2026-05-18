using Microsoft.Extensions.DependencyInjection;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Application.Services;

namespace VehicleX.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISalesManagementService, SalesManagementService>();
        services.AddScoped<IVendorService,          VendorService>();
        services.AddScoped<IPartService,            PartService>();
        services.AddScoped<ICustomerService,        CustomerService>();
        services.AddScoped<IStaffService,           StaffService>();
        services.AddScoped<IFinancialReportService, FinancialReportService>();
        services.AddScoped<ICustomerReportService,  CustomerReportService>();
        services.AddScoped<ISalesManagementService, SalesManagementService>();
        services.AddScoped<IPurchaseService,        PurchaseService>();
        services.AddScoped<INotificationService,    NotificationService>();
        return services;
    }
}

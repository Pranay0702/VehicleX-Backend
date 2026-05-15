namespace VehicleX.Application.Interfaces.Services;

public interface INotificationService
{
    Task NotifyAdminLowStockAsync(CancellationToken cancellationToken = default);
    
    Task NotifyCustomersUnpaidCreditAsync(CancellationToken cancellationToken = default);
}
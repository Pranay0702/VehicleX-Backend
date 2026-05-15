namespace VehicleX.Application.Interfaces;

public interface INotificationService
{
    Task NotifyAdminLowStockAsync(CancellationToken cancellationToken = default);
    
    Task NotifyCustomersUnpaidCreditAsync(CancellationToken cancellationToken = default);
}
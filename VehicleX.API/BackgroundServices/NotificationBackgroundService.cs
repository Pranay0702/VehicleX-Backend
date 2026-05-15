using VehicleX.Application.Interfaces.Services;

namespace VehicleX.API.BackgroundServices;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24);

    public NotificationBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification background service started — runs every 24 hours.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope   = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<INotificationService>();

                _logger.LogInformation("Running scheduled notifications at {Time}.", DateTime.UtcNow);
                await service.NotifyAdminLowStockAsync(stoppingToken);
                await service.NotifyCustomersUnpaidCreditAsync(stoppingToken);
                _logger.LogInformation("Scheduled notifications completed.");
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scheduled notifications.");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
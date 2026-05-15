using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger              = logger;
    }

    // POST api/notifications/low-stock
    [HttpPost("low-stock")]
    public async Task<IActionResult> TriggerLowStockAlert(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Manual low stock alert triggered.");
        await _notificationService.NotifyAdminLowStockAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "Low stock alert emails sent to admin."));
    }

    // POST api/notifications/unpaid-credits
    [HttpPost("unpaid-credits")]
    public async Task<IActionResult> TriggerUnpaidCreditReminders(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Manual unpaid credit reminders triggered.");
        await _notificationService.NotifyCustomersUnpaidCreditAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "Unpaid credit reminders sent."));
    }
}
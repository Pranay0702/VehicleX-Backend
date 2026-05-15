using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VehicleX.Application.DTOs.Email;
using VehicleX.Application.Interfaces;

namespace VehicleX.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly IPartRepository _partRepository;
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        IPartRepository partRepository,
        ISalesInvoiceRepository salesInvoiceRepository,
        IConfiguration configuration,
        ILogger<NotificationService> logger)
    {
        _emailService           = emailService;
        _partRepository         = partRepository;
        _salesInvoiceRepository = salesInvoiceRepository;
        _configuration          = configuration;
        _logger                 = logger;
    }

    public async Task NotifyAdminLowStockAsync(CancellationToken cancellationToken = default)
    {
        var adminEmail = _configuration["AdminSettings:Email"];
        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            _logger.LogWarning("AdminSettings:Email not configured. Skipping low stock notification.");
            return;
        }

        var allParts = await _partRepository.GetAllAsync();

        var lowStock = allParts
            .Where(p => p.StockQuantity < 10)
            .Select(p => new LowStockItemDto
            {
                PartName     = p.Name,
                PartNumber   = p.PartNumber,
                CurrentStock = p.StockQuantity
            })
            .ToList();

        if (!lowStock.Any())
        {
            _logger.LogInformation("Low stock check: all parts are sufficiently stocked.");
            return;
        }

        await _emailService.SendLowStockAlertAsync(adminEmail, lowStock, cancellationToken);
        _logger.LogInformation("Low stock alert sent to {Email} for {Count} part(s).",
            adminEmail, lowStock.Count);
    }

    public async Task NotifyCustomersUnpaidCreditAsync(CancellationToken cancellationToken = default)
    {
        var allInvoices = await _salesInvoiceRepository.GetAllWithCustomerAsync();

        // Only invoices unpaid for more than 1 month
        var cutoff = DateTime.UtcNow.AddMonths(-1);

        var overdueGroups = allInvoices
            .Where(i => i.IsCredit && !i.IsCreditPaid && i.InvoiceDate <= cutoff)
            .GroupBy(i => i.Customer)
            .ToList();

        if (!overdueGroups.Any())
        {
            _logger.LogInformation("Unpaid credit check: no overdue invoices found.");
            return;
        }

        foreach (var group in overdueGroups)
        {
            var customer = group.Key;
            if (customer is null || string.IsNullOrWhiteSpace(customer.Email))
                continue;

            var unpaidList = group
                .Select(i => new UnpaidInvoiceDto
                {
                    InvoiceId   = i.Id,
                    InvoiceDate = i.InvoiceDate,
                    Amount      = i.TotalAmount
                })
                .ToList();

            var totalOwed = unpaidList.Sum(u => u.Amount);

            await _emailService.SendUnpaidCreditReminderAsync(
                customer.Email, customer.FullName,
                totalOwed, unpaidList, cancellationToken);

            _logger.LogInformation(
                "Reminder sent to {Email} — £{Amount} across {Count} invoice(s).",
                customer.Email, totalOwed, unpaidList.Count);
        }
    }
}
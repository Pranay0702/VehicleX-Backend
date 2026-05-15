using VehicleX.Application.DTOs.Email;

namespace VehicleX.Application.Interfaces;

public interface IEmailService
{
    Task SendSalesInvoiceAsync(
        string toEmail, string customerName,
        string invoiceNumber, decimal totalAmount,
        DateTime invoiceDate, List<InvoiceItemEmailDto> items,
        CancellationToken cancellationToken = default);

    Task SendLowStockAlertAsync(
        string adminEmail, List<LowStockItemDto> lowStockItems,
        CancellationToken cancellationToken = default);

    Task SendUnpaidCreditReminderAsync(
        string toEmail, string customerName,
        decimal totalOwed, List<UnpaidInvoiceDto> unpaidInvoices,
        CancellationToken cancellationToken = default);
}
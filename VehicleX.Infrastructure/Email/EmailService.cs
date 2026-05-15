using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using VehicleX.Application.DTOs.Email;
using VehicleX.Application.Interfaces;

namespace VehicleX.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendSalesInvoiceAsync(
        string toEmail, string customerName,
        string invoiceNumber, decimal totalAmount,
        DateTime invoiceDate, List<InvoiceItemEmailDto> items,
        CancellationToken cancellationToken = default)
    {
        var body = BuildInvoiceHtml(customerName, invoiceNumber, invoiceDate, totalAmount, items);
        await SendAsync(toEmail, customerName,
            $"VehicleX — Your Invoice {invoiceNumber}", body, cancellationToken);
    }

    public async Task SendLowStockAlertAsync(
        string adminEmail, List<LowStockItemDto> items,
        CancellationToken cancellationToken = default)
    {
        var body = BuildLowStockHtml(items);
        await SendAsync(adminEmail, "Admin",
            "⚠️ VehicleX — Low Stock Alert", body, cancellationToken);
    }

    public async Task SendUnpaidCreditReminderAsync(
        string toEmail, string customerName,
        decimal totalOwed, List<UnpaidInvoiceDto> invoices,
        CancellationToken cancellationToken = default)
    {
        var body = BuildCreditReminderHtml(customerName, totalOwed, invoices);
        await SendAsync(toEmail, customerName,
            "VehicleX — Payment Reminder", body, cancellationToken);
    }

    private async Task SendAsync(
        string toEmail, string toName,
        string subject, string htmlBody,
        CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body    = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort,
            SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private static string BuildInvoiceHtml(string customerName, string invoiceNumber,
        DateTime date, decimal total, List<InvoiceItemEmailDto> items)
    {
        var rows = string.Join("", items.Select(i =>
            $"<tr>" +
            $"<td style='padding:8px;border:1px solid #ddd'>{i.PartName}</td>" +
            $"<td style='padding:8px;border:1px solid #ddd'>{i.PartNumber}</td>" +
            $"<td style='padding:8px;border:1px solid #ddd;text-align:center'>{i.Quantity}</td>" +
            $"<td style='padding:8px;border:1px solid #ddd;text-align:right'>£{i.UnitPrice:F2}</td>" +
            $"<td style='padding:8px;border:1px solid #ddd;text-align:right'>£{i.LineTotal:F2}</td>" +
            $"</tr>"));

        return $"""
            <html><body style="font-family:Arial,sans-serif;color:#333;max-width:700px;margin:auto">
            <div style="background:#1a73e8;padding:20px;text-align:center">
              <h1 style="color:white;margin:0">VehicleX</h1>
              <p style="color:#e8f0fe;margin:4px 0">Invoice Receipt</p>
            </div>
            <div style="padding:20px">
              <p>Dear <strong>{customerName}</strong>,</p>
              <p>Thank you for your purchase. Here are your invoice details.</p>
              <table style="width:100%;border-collapse:collapse;margin:10px 0">
                <tr><td><strong>Invoice:</strong> {invoiceNumber}</td>
                    <td><strong>Date:</strong> {date:dd MMM yyyy}</td></tr>
              </table>
              <table style="width:100%;border-collapse:collapse">
                <thead><tr style="background:#1a73e8;color:white">
                  <th style="padding:10px;text-align:left">Part Name</th>
                  <th style="padding:10px;text-align:left">Part #</th>
                  <th style="padding:10px;text-align:center">Qty</th>
                  <th style="padding:10px;text-align:right">Unit Price</th>
                  <th style="padding:10px;text-align:right">Total</th>
                </tr></thead>
                <tbody>{rows}</tbody>
                <tfoot><tr style="background:#f5f5f5;font-weight:bold">
                  <td colspan="4" style="padding:10px;text-align:right;border:1px solid #ddd">Grand Total</td>
                  <td style="padding:10px;text-align:right;border:1px solid #ddd">£{total:F2}</td>
                </tr></tfoot>
              </table>
              <p style="margin-top:20px">Best regards,<br/><strong>VehicleX Team</strong></p>
            </div></body></html>
            """;
    }

    private static string BuildLowStockHtml(List<LowStockItemDto> items)
    {
        var rows = string.Join("", items.Select(i =>
            $"<tr>" +
            $"<td style='padding:8px;border:1px solid #ddd'>{i.PartName}</td>" +
            $"<td style='padding:8px;border:1px solid #ddd'>{i.PartNumber}</td>" +
            $"<td style='padding:8px;border:1px solid #ddd;text-align:center;color:red'><strong>{i.CurrentStock}</strong></td>" +
            $"</tr>"));

        return $"""
            <html><body style="font-family:Arial,sans-serif;color:#333;max-width:600px;margin:auto">
            <div style="background:#e53935;padding:20px;text-align:center">
              <h1 style="color:white;margin:0">⚠️ Low Stock Alert</h1>
            </div>
            <div style="padding:20px">
              <p>The following parts have fallen <strong>below 10 units</strong>. Please restock urgently.</p>
              <table style="width:100%;border-collapse:collapse">
                <thead><tr style="background:#e53935;color:white">
                  <th style="padding:10px;text-align:left">Part Name</th>
                  <th style="padding:10px;text-align:left">Part #</th>
                  <th style="padding:10px;text-align:center">Stock</th>
                </tr></thead>
                <tbody>{rows}</tbody>
              </table>
              <p style="margin-top:20px"><strong>VehicleX System</strong></p>
            </div></body></html>
            """;
    }

    private static string BuildCreditReminderHtml(string customerName,
        decimal totalOwed, List<UnpaidInvoiceDto> invoices)
    {
        var rows = string.Join("", invoices.Select(i =>
            $"<tr>" +
            $"<td style='padding:8px;border:1px solid #ddd'>INV-{i.InvoiceId:D6}</td>" +
            $"<td style='padding:8px;border:1px solid #ddd'>{i.InvoiceDate:dd MMM yyyy}</td>" +
            $"<td style='padding:8px;border:1px solid #ddd;text-align:right'>£{i.Amount:F2}</td>" +
            $"</tr>"));

        return $"""
            <html><body style="font-family:Arial,sans-serif;color:#333;max-width:600px;margin:auto">
            <div style="background:#f57c00;padding:20px;text-align:center">
              <h1 style="color:white;margin:0">Payment Reminder</h1>
            </div>
            <div style="padding:20px">
              <p>Dear <strong>{customerName}</strong>,</p>
              <p>You have an outstanding balance of <strong>£{totalOwed:F2}</strong> overdue for more than 1 month.</p>
              <table style="width:100%;border-collapse:collapse">
                <thead><tr style="background:#f57c00;color:white">
                  <th style="padding:10px;text-align:left">Invoice #</th>
                  <th style="padding:10px;text-align:left">Date</th>
                  <th style="padding:10px;text-align:right">Amount</th>
                </tr></thead>
                <tbody>{rows}</tbody>
                <tfoot><tr style="background:#fff3e0;font-weight:bold">
                  <td colspan="2" style="padding:10px;text-align:right;border:1px solid #ddd">Total Owed</td>
                  <td style="padding:10px;text-align:right;border:1px solid #ddd">£{totalOwed:F2}</td>
                </tr></tfoot>
              </table>
              <p style="margin-top:15px">Please settle your balance at your earliest convenience.</p>
              <p><strong>VehicleX Team</strong></p>
            </div></body></html>
            """;
    }
}
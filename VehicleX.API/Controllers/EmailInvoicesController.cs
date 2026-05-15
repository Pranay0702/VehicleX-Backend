using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Email;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.API.Controllers;

[ApiController]
[Route("api/email")]
public class EmailInvoicesController : ControllerBase
{
    private readonly IEmailService          _emailService;
    private readonly ISalesInvoiceRepository _salesRepo;
    private readonly IPartRepository         _partRepo;
    private readonly ILogger<EmailInvoicesController> _logger;

    public EmailInvoicesController(
        IEmailService emailService,
        ISalesInvoiceRepository salesRepo,
        IPartRepository partRepo,
        ILogger<EmailInvoicesController> logger)
    {
        _emailService = emailService;
        _salesRepo    = salesRepo;
        _partRepo     = partRepo;
        _logger       = logger;
    }

    // POST api/email/invoice/{invoiceId}
    [HttpPost("invoice/{invoiceId:int}")]
    public async Task<IActionResult> SendInvoiceEmail(
        int invoiceId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending invoice email for invoice {InvoiceId}", invoiceId);

        var invoice = await _salesRepo.GetByIdWithItemsAsync(invoiceId, cancellationToken);
        if (invoice is null)
            return NotFound(ApiResponse<object>.Fail($"Invoice #{invoiceId} not found."));

        if (string.IsNullOrWhiteSpace(invoice.Customer?.Email))
            return BadRequest(ApiResponse<object>.Fail(
                "Customer does not have an email address on record."));

        // Build item list with part names
        var emailItems = new List<InvoiceItemEmailDto>();
        foreach (var item in invoice.Items)
        {
            var part = await _partRepo.GetByIdAsync(item.PartId);
            emailItems.Add(new InvoiceItemEmailDto
            {
                PartName   = part?.Name       ?? "Unknown Part",
                PartNumber = part?.PartNumber  ?? "-",
                Quantity   = item.Quantity,
                UnitPrice  = item.UnitPrice,
                LineTotal  = item.UnitPrice * item.Quantity
            });
        }

        var invoiceNumber = $"INV-{invoice.Id:D6}";

        await _emailService.SendSalesInvoiceAsync(
            invoice.Customer.Email,
            invoice.Customer.FullName,
            invoiceNumber,
            invoice.TotalAmount,
            invoice.InvoiceDate,
            emailItems,
            cancellationToken);

        _logger.LogInformation("Invoice {InvoiceNumber} emailed to {Email}",
            invoiceNumber, invoice.Customer.Email);

        return Ok(ApiResponse<object>.Ok(
            new { invoiceNumber, sentTo = invoice.Customer.Email },
            $"Invoice {invoiceNumber} sent to {invoice.Customer.Email}."));
    }
}
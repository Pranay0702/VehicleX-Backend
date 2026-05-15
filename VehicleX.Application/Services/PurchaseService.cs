using Microsoft.Extensions.Logging;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Purchase;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IPartRepository _partRepository;
    private readonly ILogger<PurchaseService> _logger;

    public PurchaseService(
        IPurchaseRepository purchaseRepository,
        IPartRepository partRepository,
        ILogger<PurchaseService> logger)
    {
        _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(purchaseRepository));
        _partRepository = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiResponse<IEnumerable<PurchaseResponseDto>>> GetAllPurchasesAsync()
    {
        _logger.LogInformation("Retrieving all purchase invoices.");

        var purchases = await _purchaseRepository.GetAllAsync();
        var purchaseDtos = purchases.Select(MapToResponseDto);

        _logger.LogInformation("Successfully retrieved {Count} purchase invoices.", purchaseDtos.Count());
        return ApiResponse<IEnumerable<PurchaseResponseDto>>.SuccessResponse(
            purchaseDtos, "Purchase invoices retrieved successfully.");
    }

    public async Task<ApiResponse<PurchaseResponseDto>> GetPurchaseByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving purchase invoice with ID {PurchaseId}.", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid purchase ID: {PurchaseId}.", id);
            return ApiResponse<PurchaseResponseDto>.FailureResponse(
                "Invalid purchase ID. ID must be a positive number.");
        }

        var purchase = await _purchaseRepository.GetByIdAsync(id);

        if (purchase == null)
        {
            _logger.LogWarning("Purchase invoice with ID {PurchaseId} not found.", id);
            return ApiResponse<PurchaseResponseDto>.FailureResponse(
                $"Purchase invoice with ID {id} not found.");
        }

        _logger.LogInformation("Successfully retrieved purchase invoice with ID {PurchaseId}.", id);
        return ApiResponse<PurchaseResponseDto>.SuccessResponse(
            MapToResponseDto(purchase), "Purchase invoice retrieved successfully.");
    }

    public async Task<ApiResponse<PurchaseResponseDto>> CreatePurchaseAsync(CreatePurchaseDto dto)
    {
        _logger.LogInformation("Creating new purchase invoice with {ItemCount} items.", dto.Items.Count);

        var errors = new List<string>();
        var partsToUpdate = new List<Part>();
        var purchaseItems = new List<PurchaseInvoiceItem>();
        decimal totalAmount = 0;

        // Check for duplicate part IDs in the same invoice
        var duplicatePartIds = dto.Items
            .GroupBy(i => i.PartId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicatePartIds.Any())
        {
            errors.Add($"Duplicate part IDs found in the invoice: {string.Join(", ", duplicatePartIds)}. " +
                        "Please combine quantities for the same part into a single line item.");
        }

        if (errors.Any())
        {
            _logger.LogWarning("Purchase validation failed: {Errors}", string.Join("; ", errors));
            return ApiResponse<PurchaseResponseDto>.FailureResponse(
                "Purchase validation failed.", errors);
        }

        // Validate each item
        for (int i = 0; i < dto.Items.Count; i++)
        {
            var item = dto.Items[i];
            var itemLabel = $"Item #{i + 1} (Part ID: {item.PartId})";

            // Validate part exists
            var part = await _partRepository.GetByIdAsync(item.PartId);

            if (part == null)
            {
                errors.Add($"{itemLabel}: Part with ID {item.PartId} does not exist.");
                continue;
            }

            // Validate quantity
            if (item.Quantity <= 0)
            {
                errors.Add($"{itemLabel}: Quantity must be at least 1.");
                continue;
            }

            // Validate unit price
            if (item.UnitPrice <= 0)
            {
                errors.Add($"{itemLabel}: Unit price must be greater than zero.");
                continue;
            }

            // Calculate line item total
            decimal lineTotal = item.Quantity * item.UnitPrice;
            totalAmount += lineTotal;

            // Update stock quantity (increase for purchase)
            part.StockQuantity += item.Quantity;
            part.UpdatedAt = DateTime.UtcNow;
            partsToUpdate.Add(part);

            // Create purchase item
            purchaseItems.Add(new PurchaseInvoiceItem
            {
                PartId = item.PartId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = lineTotal,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        if (errors.Any())
        {
            _logger.LogWarning("Purchase validation failed with {ErrorCount} errors.", errors.Count);
            return ApiResponse<PurchaseResponseDto>.FailureResponse(
                "Purchase validation failed. Please fix the following errors:", errors);
        }

        // Generate invoice number

        var todaysCount = await _purchaseRepository.GetTodaysPurchaseCountAsync();
        var invoiceNumber = $"PUR-{DateTime.UtcNow:yyyyMMdd}-{(todaysCount + 1):D3}";

        // Create purchase invoice

        var invoice = new PurchaseInvoice
        {
            InvoiceNumber = invoiceNumber,
            PurchaseDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Notes = dto.Notes?.Trim(),
            Items = purchaseItems,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdInvoice = await _purchaseRepository.CreatePurchaseAsync(invoice, partsToUpdate);

        var reloadedInvoice = await _purchaseRepository.GetByIdAsync(createdInvoice.Id);

        _logger.LogInformation(
            "Successfully created purchase invoice {InvoiceNumber} with {ItemCount} items. Total: {Total:C}",
            invoiceNumber, purchaseItems.Count, totalAmount);

        return ApiResponse<PurchaseResponseDto>.SuccessResponse(
            MapToResponseDto(reloadedInvoice!),
            $"Purchase invoice {invoiceNumber} created successfully. Total amount: {totalAmount:C}.");
    }

    private static PurchaseResponseDto MapToResponseDto(PurchaseInvoice invoice)
    {
        return new PurchaseResponseDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PurchaseDate = invoice.PurchaseDate,
            TotalAmount = invoice.TotalAmount,
            Notes = invoice.Notes,
            CreatedAt = invoice.CreatedAt,
            Items = invoice.Items?.Select(item => new PurchaseItemResponseDto
            {
                Id = item.Id,
                PartId = item.PartId,
                PartName = item.Part?.Name ?? "Unknown",
                PartNumber = item.Part?.PartNumber ?? "N/A",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList() ?? new List<PurchaseItemResponseDto>()
        };
    }
}

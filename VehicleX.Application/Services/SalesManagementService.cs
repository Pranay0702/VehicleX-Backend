using Microsoft.Extensions.Logging;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Sales;
using VehicleX.Application.DTOs.Shared;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class SalesManagementService : ISalesManagementService
{
    private const decimal LoyaltyDiscountThreshold = 5000m;
    private const decimal LoyaltyDiscountRate = 0.10m;

    private readonly ICustomerRepository _customerRepository;
    private readonly IPartRepository _partRepository;
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILogger<SalesManagementService> _logger;

    public SalesManagementService(
        ICustomerRepository customerRepository,
        IPartRepository partRepository,
        ISalesInvoiceRepository salesInvoiceRepository,
        IRepositoryManager repositoryManager,
        ILogger<SalesManagementService> logger)
    {
        _customerRepository = customerRepository;
        _partRepository = partRepository;
        _salesInvoiceRepository = salesInvoiceRepository;
        _repositoryManager = repositoryManager;
        _logger = logger;
    }

    public async Task<ApiResponse<List<CustomerLookupDto>>> GetCustomersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var customers = await _customerRepository.GetAllAsync(cancellationToken);

            var response = customers
                .Select(customer => new CustomerLookupDto
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    Email = customer.Email
                })
                .ToList();

            return ApiResponse<List<CustomerLookupDto>>.Ok(response, "Customers fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching customers.");
            return ApiResponse<List<CustomerLookupDto>>.Fail("Unable to fetch customers right now.");
        }
    }

    public async Task<ApiResponse<List<PartLookupDto>>> GetPartsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var parts = await _partRepository.GetAllAsync();

            var response = parts
                .Select(part => new PartLookupDto
                {
                    Id = part.Id,
                    Name = part.Name,
                    PartNumber = part.PartNumber,
                    UnitPrice = part.Price,
                    StockQuantity = part.StockQuantity
                })
                .ToList();

            return ApiResponse<List<PartLookupDto>>.Ok(response, "Parts fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching parts.");
            return ApiResponse<List<PartLookupDto>>.Fail("Unable to fetch parts right now.");
        }
    }

    public async Task<ApiResponse<SalesInvoiceResponseDto>> CreateSalesInvoiceAsync(
        CreateSalesInvoiceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.Items is null || request.Items.Count == 0)
            {
                return ApiResponse<SalesInvoiceResponseDto>.Fail(
                    "At least one sales item is required.");
            }

            var duplicatePartIds = request.Items
                .GroupBy(item => item.PartId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicatePartIds.Count > 0)
            {
                return ApiResponse<SalesInvoiceResponseDto>.Fail(
                    $"Duplicate part entries are not allowed. Duplicate Part IDs: {string.Join(", ", duplicatePartIds)}");
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return ApiResponse<SalesInvoiceResponseDto>.Fail("Customer not found.");
            }

            var requestedPartIds = request.Items.Select(item => item.PartId).Distinct().ToList();
            var partLookup = new Dictionary<int, Part>();
            var missingPartIds = new List<int>();
            foreach (var partId in requestedPartIds)
            {
                var part = await _partRepository.GetByIdAsync(partId);
                if (part is null)
                {
                    missingPartIds.Add(partId);
                    continue;
                }

                partLookup[partId] = part;
            }

            if (missingPartIds.Count > 0)
            {
                return ApiResponse<SalesInvoiceResponseDto>.Fail(
                    $"Part(s) not found: {string.Join(", ", missingPartIds)}.");
            }

            foreach (var requestedItem in request.Items)
            {
                var part = partLookup[requestedItem.PartId];

                if (requestedItem.Quantity <= 0)
                {
                    return ApiResponse<SalesInvoiceResponseDto>.Fail(
                        $"Quantity for part '{part.Name}' must be greater than 0.");
                }

                if (part.Price < 0)
                {
                    return ApiResponse<SalesInvoiceResponseDto>.Fail(
                        $"Part '{part.Name}' has invalid unit price configured.");
                }

                if (part.StockQuantity < requestedItem.Quantity)
                {
                    return ApiResponse<SalesInvoiceResponseDto>.Fail(
                        $"Insufficient stock for part '{part.Name}'. Available: {part.StockQuantity}, Requested: {requestedItem.Quantity}.");
                }
            }

            var invoice = new SalesInvoice
            {
                CustomerId = customer.Id,
                InvoiceDate = DateTime.UtcNow,
                IsCredit = false,
                IsCreditPaid = false
            };

            decimal subtotal = 0m;

            foreach (var requestedItem in request.Items)
            {
                var part = partLookup[requestedItem.PartId];

                part.StockQuantity -= requestedItem.Quantity;

                var lineTotal = part.Price * requestedItem.Quantity;
                subtotal += lineTotal;

                invoice.Items.Add(new SalesInvoiceItem
                {
                    PartId = part.Id,
                    Quantity = requestedItem.Quantity,
                    UnitPrice = part.Price
                });
            }

            var discountAmount = CalculateLoyaltyDiscount(subtotal);
            invoice.TotalAmount = CalculateFinalPayableAmount(subtotal, discountAmount);

            await _salesInvoiceRepository.AddAsync(invoice, cancellationToken);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            var invoiceNumber = BuildInvoiceNumber(invoice.Id);
            var response = new SalesInvoiceResponseDto
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoiceNumber,
                CreatedAtUtc = invoice.InvoiceDate,
                CustomerId = customer.Id,
                CustomerName = customer.FullName,
                SubTotalAmount = subtotal,
                DiscountAmount = discountAmount,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items
                    .Select(item => new SalesInvoiceItemResponseDto
                    {
                        PartId = item.PartId,
                        PartName = partLookup[item.PartId].Name,
                        PartNumber = partLookup[item.PartId].PartNumber,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.UnitPrice * item.Quantity
                    })
                    .ToList()
            };
            
            return ApiResponse<SalesInvoiceResponseDto>.Ok(
                response,
                "Sales invoice created successfully and stock was updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating sales invoice for CustomerId {CustomerId}.", request.CustomerId);
            return ApiResponse<SalesInvoiceResponseDto>.Fail("Unable to create sales invoice right now.");
        }
    }

    public async Task<ApiResponse<SalesInvoiceResponseDto>> GetSalesInvoiceByIdAsync(
        int invoiceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (invoiceId <= 0)
            {
                return ApiResponse<SalesInvoiceResponseDto>.Fail(
                    "InvoiceId must be greater than 0.");
            }

            var invoice = await _salesInvoiceRepository.GetByIdWithItemsAsync(invoiceId, cancellationToken);

            if (invoice is null)
            {
                return ApiResponse<SalesInvoiceResponseDto>.Fail("Sales invoice not found.");
            }

            var partLookup = new Dictionary<int, Part>();
            foreach (var partId in invoice.Items.Select(item => item.PartId).Distinct())
            {
                var part = await _partRepository.GetByIdAsync(partId);
                if (part is not null)
                {
                    partLookup[partId] = part;
                }
            }

            var subtotal = invoice.Items.Sum(item => item.UnitPrice * item.Quantity);
            var discountAmount = CalculateStoredDiscountAmount(subtotal, invoice.TotalAmount);
            var response = new SalesInvoiceResponseDto
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = BuildInvoiceNumber(invoice.Id),
                CreatedAtUtc = invoice.InvoiceDate,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer?.FullName ?? string.Empty,
                SubTotalAmount = subtotal,
                DiscountAmount = discountAmount,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items
                    .Select(item => new SalesInvoiceItemResponseDto
                    {
                        PartId = item.PartId,
                        PartName = partLookup.TryGetValue(item.PartId, out var part) ? part.Name : string.Empty,
                        PartNumber = partLookup.TryGetValue(item.PartId, out part) ? part.PartNumber : string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.UnitPrice * item.Quantity
                    })
                    .ToList()
            };

            return ApiResponse<SalesInvoiceResponseDto>.Ok(response, "Sales invoice fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching invoice {InvoiceId}.", invoiceId);
            return ApiResponse<SalesInvoiceResponseDto>.Fail("Unable to fetch sales invoice right now.");
        }
    }

    private static string BuildInvoiceNumber(int invoiceId)
    {
        return $"INV-{invoiceId:D6}";
    }

    private static decimal CalculateLoyaltyDiscount(decimal subtotal)
    {
        if (subtotal <= LoyaltyDiscountThreshold)
        {
            return 0m;
        }

        return decimal.Round(subtotal * LoyaltyDiscountRate, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal CalculateFinalPayableAmount(decimal subtotal, decimal discountAmount)
    {
        var finalAmount = subtotal - discountAmount;
        return finalAmount < 0m ? 0m : finalAmount;
    }

    private static decimal CalculateStoredDiscountAmount(decimal subtotal, decimal totalAmount)
    {
        if (subtotal <= totalAmount)
        {
            return 0m;
        }

        return subtotal - totalAmount;
    }
}

using Microsoft.Extensions.Logging;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class SalesManagementService : ISalesManagementService
{
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

    public async Task<ServiceResult<List<CustomerLookupDto>>> GetCustomersAsync(CancellationToken cancellationToken = default)
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

            return ServiceResult<List<CustomerLookupDto>>.Success(response, "Customers fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching customers.");
            return ServiceResult<List<CustomerLookupDto>>.Failure("Unable to fetch customers right now.");
        }
    }

    public async Task<ServiceResult<List<PartLookupDto>>> GetPartsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var parts = await _partRepository.GetAllAsync(cancellationToken);

            var response = parts
                .Select(part => new PartLookupDto
                {
                    Id = part.Id,
                    Name = part.Name,
                    PartNumber = part.PartNumber,
                    UnitPrice = part.UnitPrice,
                    StockQuantity = part.StockQuantity
                })
                .ToList();

            return ServiceResult<List<PartLookupDto>>.Success(response, "Parts fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching parts.");
            return ServiceResult<List<PartLookupDto>>.Failure("Unable to fetch parts right now.");
        }
    }

    public async Task<ServiceResult<SalesInvoiceResponseDto>> CreateSalesInvoiceAsync(
        CreateSalesInvoiceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.Items.Count == 0)
            {
                return ServiceResult<SalesInvoiceResponseDto>.ValidationFailure("At least one sales item is required.");
            }

            var duplicatePartIds = request.Items
                .GroupBy(item => item.PartId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicatePartIds.Count > 0)
            {
                return ServiceResult<SalesInvoiceResponseDto>.ValidationFailure(
                    $"Duplicate part entries are not allowed. Duplicate Part IDs: {string.Join(", ", duplicatePartIds)}");
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<SalesInvoiceResponseDto>.NotFound("Customer not found.");
            }

            var requestedPartIds = request.Items.Select(item => item.PartId).Distinct().ToList();
            var parts = await _partRepository.GetByIdsAsync(requestedPartIds, cancellationToken);
            var partLookup = parts.ToDictionary(part => part.Id);

            var missingPartIds = requestedPartIds
                .Where(partId => !partLookup.ContainsKey(partId))
                .ToList();

            if (missingPartIds.Count > 0)
            {
                return ServiceResult<SalesInvoiceResponseDto>.NotFound($"Part(s) not found: {string.Join(", ", missingPartIds)}.");
            }

            foreach (var requestedItem in request.Items)
            {
                var part = partLookup[requestedItem.PartId];

                if (requestedItem.Quantity <= 0)
                {
                    return ServiceResult<SalesInvoiceResponseDto>.ValidationFailure(
                        $"Quantity for part '{part.Name}' must be greater than 0.");
                }

                if (part.UnitPrice < 0)
                {
                    return ServiceResult<SalesInvoiceResponseDto>.Failure(
                        $"Part '{part.Name}' has invalid unit price configured.");
                }

                if (part.StockQuantity < requestedItem.Quantity)
                {
                    return ServiceResult<SalesInvoiceResponseDto>.Conflict(
                        $"Insufficient stock for part '{part.Name}'. Available: {part.StockQuantity}, Requested: {requestedItem.Quantity}.");
                }
            }

            var invoice = new SalesInvoice
            {
                CustomerId = customer.Id,
                InvoiceNumber = GenerateInvoiceNumber(),
                CreatedAtUtc = DateTime.UtcNow,
                DiscountAmount = 0m
            };

            decimal subtotal = 0m;

            foreach (var requestedItem in request.Items)
            {
                var part = partLookup[requestedItem.PartId];

                part.StockQuantity -= requestedItem.Quantity;

                var lineTotal = part.UnitPrice * requestedItem.Quantity;
                subtotal += lineTotal;

                invoice.Items.Add(new SalesItem
                {
                    PartId = part.Id,
                    Quantity = requestedItem.Quantity,
                    UnitPrice = part.UnitPrice,
                    LineTotal = lineTotal
                });
            }

            invoice.SubTotalAmount = subtotal;
            invoice.TotalAmount = subtotal;

            await _salesInvoiceRepository.AddAsync(invoice, cancellationToken);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            var response = new SalesInvoiceResponseDto
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CreatedAtUtc = invoice.CreatedAtUtc,
                CustomerId = customer.Id,
                CustomerName = customer.FullName,
                SubTotalAmount = invoice.SubTotalAmount,
                DiscountAmount = invoice.DiscountAmount,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items
                    .Select(item => new SalesInvoiceItemResponseDto
                    {
                        PartId = item.PartId,
                        PartName = partLookup[item.PartId].Name,
                        PartNumber = partLookup[item.PartId].PartNumber,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.LineTotal
                    })
                    .ToList()
            };

            return ServiceResult<SalesInvoiceResponseDto>.Success(
                response,
                "Sales invoice created successfully and stock was updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating sales invoice for CustomerId {CustomerId}.", request.CustomerId);
            return ServiceResult<SalesInvoiceResponseDto>.Failure("Unable to create sales invoice right now.");
        }
    }

    public async Task<ServiceResult<SalesInvoiceResponseDto>> GetSalesInvoiceByIdAsync(
        int invoiceId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (invoiceId <= 0)
            {
                return ServiceResult<SalesInvoiceResponseDto>.ValidationFailure("InvoiceId must be greater than 0.");
            }

            var invoice = await _salesInvoiceRepository.GetByIdWithItemsAsync(invoiceId, cancellationToken);

            if (invoice is null)
            {
                return ServiceResult<SalesInvoiceResponseDto>.NotFound("Sales invoice not found.");
            }

            var response = new SalesInvoiceResponseDto
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CreatedAtUtc = invoice.CreatedAtUtc,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer?.FullName ?? string.Empty,
                SubTotalAmount = invoice.SubTotalAmount,
                DiscountAmount = invoice.DiscountAmount,
                TotalAmount = invoice.TotalAmount,
                Items = invoice.Items
                    .Select(item => new SalesInvoiceItemResponseDto
                    {
                        PartId = item.PartId,
                        PartName = item.Part?.Name ?? string.Empty,
                        PartNumber = item.Part?.PartNumber ?? string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.LineTotal
                    })
                    .ToList()
            };

            return ServiceResult<SalesInvoiceResponseDto>.Success(response, "Sales invoice fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching invoice {InvoiceId}.", invoiceId);
            return ServiceResult<SalesInvoiceResponseDto>.Failure("Unable to fetch sales invoice right now.");
        }
    }

    private static string GenerateInvoiceNumber()
    {
        var randomSegment = Random.Shared.Next(1000, 9999);
        return $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{randomSegment}";
    }
}

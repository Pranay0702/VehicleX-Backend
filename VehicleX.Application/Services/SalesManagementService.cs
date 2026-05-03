using System.Net;
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

            return ServiceResult<List<CustomerLookupDto>>.Ok(response, "Customers fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching customers.");
            return ServiceResult<List<CustomerLookupDto>>.Fail("Unable to fetch customers right now.", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<List<PartLookupDto>>> GetPartsAsync(CancellationToken cancellationToken = default)
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

            return ServiceResult<List<PartLookupDto>>.Ok(response, "Parts fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching parts.");
            return ServiceResult<List<PartLookupDto>>.Fail("Unable to fetch parts right now.", (int)HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<SalesInvoiceResponseDto>> CreateSalesInvoiceAsync(
        CreateSalesInvoiceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.Items is null || request.Items.Count == 0)
            {
                return ServiceResult<SalesInvoiceResponseDto>.Fail(
                    "At least one sales item is required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var duplicatePartIds = request.Items
                .GroupBy(item => item.PartId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicatePartIds.Count > 0)
            {
                return ServiceResult<SalesInvoiceResponseDto>.Fail(
                    $"Duplicate part entries are not allowed. Duplicate Part IDs: {string.Join(", ", duplicatePartIds)}",
                    (int)HttpStatusCode.BadRequest);
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<SalesInvoiceResponseDto>.Fail("Customer not found.", (int)HttpStatusCode.NotFound);
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
                return ServiceResult<SalesInvoiceResponseDto>.Fail(
                    $"Part(s) not found: {string.Join(", ", missingPartIds)}.",
                    (int)HttpStatusCode.NotFound);
            }

            foreach (var requestedItem in request.Items)
            {
                var part = partLookup[requestedItem.PartId];

                if (requestedItem.Quantity <= 0)
                {
                    return ServiceResult<SalesInvoiceResponseDto>.Fail(
                        $"Quantity for part '{part.Name}' must be greater than 0.",
                        (int)HttpStatusCode.BadRequest);
                }

                if (part.Price < 0)
                {
                    return ServiceResult<SalesInvoiceResponseDto>.Fail(
                        $"Part '{part.Name}' has invalid unit price configured.",
                        (int)HttpStatusCode.InternalServerError);
                }

                if (part.StockQuantity < requestedItem.Quantity)
                {
                    return ServiceResult<SalesInvoiceResponseDto>.Fail(
                        $"Insufficient stock for part '{part.Name}'. Available: {part.StockQuantity}, Requested: {requestedItem.Quantity}.",
                        (int)HttpStatusCode.Conflict);
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

            invoice.TotalAmount = subtotal;

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
                DiscountAmount = 0m,
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

            return ServiceResult<SalesInvoiceResponseDto>.Ok(
                response,
                "Sales invoice created successfully and stock was updated.",
                (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating sales invoice for CustomerId {CustomerId}.", request.CustomerId);
            return ServiceResult<SalesInvoiceResponseDto>.Fail("Unable to create sales invoice right now.", (int)HttpStatusCode.InternalServerError);
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
                return ServiceResult<SalesInvoiceResponseDto>.Fail(
                    "InvoiceId must be greater than 0.",
                    (int)HttpStatusCode.BadRequest);
            }

            var invoice = await _salesInvoiceRepository.GetByIdWithItemsAsync(invoiceId, cancellationToken);

            if (invoice is null)
            {
                return ServiceResult<SalesInvoiceResponseDto>.Fail("Sales invoice not found.", (int)HttpStatusCode.NotFound);
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
            var response = new SalesInvoiceResponseDto
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = BuildInvoiceNumber(invoice.Id),
                CreatedAtUtc = invoice.InvoiceDate,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer?.FullName ?? string.Empty,
                SubTotalAmount = subtotal,
                DiscountAmount = 0m,
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

            return ServiceResult<SalesInvoiceResponseDto>.Ok(response, "Sales invoice fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching invoice {InvoiceId}.", invoiceId);
            return ServiceResult<SalesInvoiceResponseDto>.Fail("Unable to fetch sales invoice right now.", (int)HttpStatusCode.InternalServerError);
        }
    }

    private static string BuildInvoiceNumber(int invoiceId)
    {
        return $"INV-{invoiceId:D6}";
    }
}

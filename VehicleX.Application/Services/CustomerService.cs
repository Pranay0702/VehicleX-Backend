using Microsoft.Extensions.Logging;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Customers;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ISalesInvoiceRepository _salesInvoiceRepository;
    private readonly IPartRepository _partRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        ISalesInvoiceRepository salesInvoiceRepository,
        IPartRepository partRepository,
        IJwtTokenService jwtTokenService,
        ILogger<CustomerService> logger)
    {
        _customerRepository    = customerRepository;
        _salesInvoiceRepository = salesInvoiceRepository;
        _partRepository        = partRepository;
        _jwtTokenService       = jwtTokenService;
        _logger                = logger;
    }

    // -------------------------------------------------------------------------
    // Registration & Auth
    // -------------------------------------------------------------------------

    public async Task<ApiResponse<CustomerResponseDto>> StaffRegisterCustomerAsync(StaffRegisterCustomerDto dto)
    {
        if (await _customerRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
            return ApiResponse<CustomerResponseDto>.Fail("Phone number already exists.");

        if (await _customerRepository.EmailExistsAsync(NormalizeEmail(dto.Email)))
            return ApiResponse<CustomerResponseDto>.Fail("Email already exists.");

        if (dto.Vehicle == null)
            return ApiResponse<CustomerResponseDto>.Fail("Vehicle details are required.");

        if (await _customerRepository.VehicleNumberExistsAsync(dto.Vehicle.VehicleNumber))
            return ApiResponse<CustomerResponseDto>.Fail("Vehicle number already exists.");

        var customer = new Customer
        {
            FullName    = dto.FullName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Email       = NormalizeEmail(dto.Email),
            Address     = dto.Address?.Trim(),
            Vehicles    = new List<Vehicle>
            {
                new Vehicle
                {
                    VehicleNumber = dto.Vehicle.VehicleNumber.Trim(),
                    Brand         = dto.Vehicle.Brand.Trim(),
                    Model         = dto.Vehicle.Model.Trim(),
                    Year          = dto.Vehicle.Year,
                    FuelType      = dto.Vehicle.FuelType?.Trim()
                }
            }
        };

        var savedCustomer = await _customerRepository.AddAsync(customer);

        return ApiResponse<CustomerResponseDto>.Ok(
            MapToCustomerResponse(savedCustomer),
            "Customer registered successfully.");
    }

    public async Task<ApiResponse<CustomerAuthResponseDto>> CustomerSelfRegisterAsync(CustomerSelfRegisterDto dto)
    {
        if (await _customerRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
            return ApiResponse<CustomerAuthResponseDto>.Fail("Phone number already exists.");

        if (await _customerRepository.EmailExistsAsync(NormalizeEmail(dto.Email)))
            return ApiResponse<CustomerAuthResponseDto>.Fail("Email already exists.");

        if (dto.Vehicle == null)
            return ApiResponse<CustomerAuthResponseDto>.Fail("Vehicle details are required.");

        if (await _customerRepository.VehicleNumberExistsAsync(dto.Vehicle.VehicleNumber))
            return ApiResponse<CustomerAuthResponseDto>.Fail("Vehicle number already exists.");

        var customer = new Customer
        {
            FullName     = dto.FullName.Trim(),
            PhoneNumber  = dto.PhoneNumber.Trim(),
            Email        = NormalizeEmail(dto.Email),
            Address      = dto.Address?.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Vehicles     = new List<Vehicle>
            {
                new Vehicle
                {
                    VehicleNumber = dto.Vehicle.VehicleNumber.Trim(),
                    Brand         = dto.Vehicle.Brand.Trim(),
                    Model         = dto.Vehicle.Model.Trim(),
                    Year          = dto.Vehicle.Year,
                    FuelType      = dto.Vehicle.FuelType?.Trim()
                }
            }
        };

        var savedCustomer = await _customerRepository.AddAsync(customer);
        var token = _jwtTokenService.GenerateCustomerToken(savedCustomer);

        var response = new CustomerAuthResponseDto
        {
            Id          = savedCustomer.Id,
            FullName    = savedCustomer.FullName,
            PhoneNumber = savedCustomer.PhoneNumber,
            Email       = savedCustomer.Email,
            Role        = "Customer",
            Token       = token
        };

        return ApiResponse<CustomerAuthResponseDto>.Ok(response, "Customer registered successfully.");
    }

    public async Task<ApiResponse<CustomerAuthResponseDto>> CustomerLoginAsync(CustomerLoginDto dto)
    {
        var customer = await _customerRepository.GetByEmailAsync(NormalizeEmail(dto.Email));

        if (customer == null || string.IsNullOrWhiteSpace(customer.PasswordHash))
            return ApiResponse<CustomerAuthResponseDto>.Fail("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
            return ApiResponse<CustomerAuthResponseDto>.Fail("Invalid email or password.");

        var token = _jwtTokenService.GenerateCustomerToken(customer);

        var response = new CustomerAuthResponseDto
        {
            Id          = customer.Id,
            FullName    = customer.FullName,
            PhoneNumber = customer.PhoneNumber,
            Email       = customer.Email,
            Role        = "Customer",
            Token       = token
        };

        return ApiResponse<CustomerAuthResponseDto>.Ok(response, "Customer logged in successfully.");
    }

    // -------------------------------------------------------------------------
    // Staff-facing queries
    // -------------------------------------------------------------------------

    public async Task<ApiResponse<List<CustomerResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);

        var response = customers
            .Select(MapToCustomerResponse)
            .ToList();

        return ApiResponse<List<CustomerResponseDto>>.Ok(response, "Customers retrieved successfully.");
    }

    public async Task<ApiResponse<CustomerResponseDto>> GetByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
            return ApiResponse<CustomerResponseDto>.Fail("CustomerId must be greater than 0.");

        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);

        if (customer is null)
            return ApiResponse<CustomerResponseDto>.Fail("Customer not found.");

        return ApiResponse<CustomerResponseDto>.Ok(MapToCustomerResponse(customer), "Customer retrieved successfully.");
    }

    public async Task<ApiResponse<List<CustomerSearchResultDto>>> SearchCustomersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return ApiResponse<List<CustomerSearchResultDto>>.Fail("Search term is required.");

        var customers = await _customerRepository.SearchCustomersAsync(searchTerm);

        var response = customers.Select(c => new CustomerSearchResultDto
        {
            Id          = c.Id,
            FullName    = c.FullName,
            PhoneNumber = c.PhoneNumber,
            Email       = c.Email,
            Address     = c.Address,
            CreatedAt   = c.CreatedAt,
            Vehicles    = c.Vehicles.Select(MapToVehicleResponse).ToList()
        }).ToList();

        return ApiResponse<List<CustomerSearchResultDto>>.Ok(
            response,
            response.Any() ? "Customers found successfully." : "No customers matched the search term.");
    }

    public async Task<ApiResponse<CustomerDetailsHistoryResponseDto>> GetCustomerDetailsAndHistoryForStaffAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (customerId <= 0)
                return ApiResponse<CustomerDetailsHistoryResponseDto>.Fail("CustomerId must be greater than 0.");

            var customer = await _customerRepository.GetByIdWithVehiclesAsync(customerId);
            if (customer is null)
                return ApiResponse<CustomerDetailsHistoryResponseDto>.Fail("Customer not found.");

            var invoices = await _salesInvoiceRepository.GetByCustomerIdWithItemsAsync(customerId, cancellationToken);
            var distinctPartIds = invoices
                .SelectMany(i => i.Items)
                .Select(i => i.PartId)
                .Distinct()
                .ToList();

            var parts = await _partRepository.GetByIdsAsync(distinctPartIds, cancellationToken);
            var partLookup = parts.ToDictionary(p => p.Id, p => p);

            var purchaseHistory = invoices
                .OrderByDescending(i => i.InvoiceDate)
                .Select(invoice =>
                {
                    var subtotal       = invoice.Items.Sum(item => item.UnitPrice * item.Quantity);
                    var discountAmount = subtotal > invoice.TotalAmount ? subtotal - invoice.TotalAmount : 0m;

                    return new CustomerPurchaseHistoryDto
                    {
                        InvoiceId      = invoice.Id,
                        InvoiceNumber  = BuildInvoiceNumber(invoice.Id),
                        InvoiceDateUtc = invoice.InvoiceDate,
                        SubTotalAmount = subtotal,
                        DiscountAmount = discountAmount,
                        TotalAmount    = invoice.TotalAmount,
                        IsCredit       = invoice.IsCredit,
                        IsCreditPaid   = invoice.IsCreditPaid,
                        Items          = invoice.Items
                            .Select(item => new CustomerPurchaseHistoryItemDto
                            {
                                PartId    = item.PartId,
                                PartName  = partLookup.TryGetValue(item.PartId, out var part) ? part.Name       : "Unknown Part",
                                PartNumber= partLookup.TryGetValue(item.PartId, out part)     ? part.PartNumber : "N/A",
                                Quantity  = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                LineTotal = item.UnitPrice * item.Quantity
                            })
                            .ToList()
                    };
                })
                .ToList();

            var response = new CustomerDetailsHistoryResponseDto
            {
                CustomerId              = customer.Id,
                FullName                = customer.FullName,
                PhoneNumber             = customer.PhoneNumber,
                Email                   = customer.Email,
                Address                 = customer.Address,
                CustomerSinceUtc        = customer.CreatedAt,
                Vehicles                = customer.Vehicles
                    .Select(MapToVehicleResponse)
                    .OrderBy(v => v.VehicleNumber)
                    .ToList(),
                PurchaseHistory         = purchaseHistory,
                IsServiceHistoryAvailable = false,
                ServiceHistory          = new List<CustomerServiceHistoryDto>()
            };

            return ApiResponse<CustomerDetailsHistoryResponseDto>.Ok(
                response, "Customer details and history fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching details/history for customer {CustomerId}.", customerId);
            return ApiResponse<CustomerDetailsHistoryResponseDto>.Fail(
                "Unable to fetch customer details and history right now.");
        }
    }

    // -------------------------------------------------------------------------
    // Customer self-service (profile & vehicles)
    // -------------------------------------------------------------------------

    public async Task<ApiResponse<CustomerResponseDto>> GetProfileAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdWithVehiclesAsync(customerId);

        if (customer == null)
            return ApiResponse<CustomerResponseDto>.Fail("Customer profile not found.");

        return ApiResponse<CustomerResponseDto>.Ok(
            MapToCustomerResponse(customer), "Customer profile loaded successfully.");
    }

    public async Task<ApiResponse<CustomerResponseDto>> UpdateProfileAsync(int customerId, UpdateCustomerProfileDto dto)
    {
        var customer = await _customerRepository.GetByIdWithVehiclesAsync(customerId);

        if (customer == null)
            return ApiResponse<CustomerResponseDto>.Fail("Customer profile not found.");

        var normalizedEmail = NormalizeEmail(dto.Email);

        if (!string.Equals(customer.PhoneNumber, dto.PhoneNumber.Trim(), StringComparison.OrdinalIgnoreCase)
            && await _customerRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
            return ApiResponse<CustomerResponseDto>.Fail("Phone number already exists.");

        // Pass excludedCustomerId so the customer's own email doesn't trigger a conflict
        if (!string.Equals(customer.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)
            && await _customerRepository.EmailExistsAsync(normalizedEmail, customerId))
            return ApiResponse<CustomerResponseDto>.Fail("Email already exists.");

        customer.FullName    = dto.FullName.Trim();
        customer.PhoneNumber = dto.PhoneNumber.Trim();
        customer.Email       = normalizedEmail;
        customer.Address     = dto.Address?.Trim();
        customer.UpdatedAt   = DateTime.UtcNow;

        var updatedCustomer = await _customerRepository.UpdateAsync(customer);

        return ApiResponse<CustomerResponseDto>.Ok(
            MapToCustomerResponse(updatedCustomer), "Profile updated successfully.");
    }

    public async Task<ApiResponse<List<VehicleResponseDto>>> GetMyVehiclesAsync(int customerId)
    {
        var vehicles = await _customerRepository.GetVehiclesByCustomerIdAsync(customerId);

        var response = vehicles.Select(MapToVehicleResponse).ToList();

        return ApiResponse<List<VehicleResponseDto>>.Ok(response, "Vehicles loaded successfully.");
    }

    public async Task<ApiResponse<VehicleResponseDto>> AddVehicleAsync(int customerId, CreateCustomerVehicleDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);

        if (customer == null)
            return ApiResponse<VehicleResponseDto>.Fail("Customer profile not found.");

        if (await _customerRepository.VehicleNumberExistsAsync(dto.VehicleNumber))
            return ApiResponse<VehicleResponseDto>.Fail("Vehicle number already exists.");

        var vehicle = new Vehicle
        {
            CustomerId    = customerId,
            VehicleNumber = dto.VehicleNumber.Trim(),
            Brand         = dto.Brand.Trim(),
            Model         = dto.Model.Trim(),
            Year          = dto.Year,
            FuelType      = dto.FuelType?.Trim()
        };

        var savedVehicle = await _customerRepository.AddVehicleAsync(vehicle);

        return ApiResponse<VehicleResponseDto>.Ok(
            MapToVehicleResponse(savedVehicle), "Vehicle added successfully.");
    }

    public async Task<ApiResponse<VehicleResponseDto>> UpdateVehicleAsync(int customerId, int vehicleId, UpdateCustomerVehicleDto dto)
    {
        var vehicle = await _customerRepository.GetVehicleByIdAndCustomerIdAsync(vehicleId, customerId);

        if (vehicle == null)
            return ApiResponse<VehicleResponseDto>.Fail("Vehicle not found.");

        if (await _customerRepository.VehicleNumberExistsForOtherCustomerAsync(dto.VehicleNumber, customerId, vehicleId))
            return ApiResponse<VehicleResponseDto>.Fail("Vehicle number already exists.");

        vehicle.VehicleNumber = dto.VehicleNumber.Trim();
        vehicle.Brand         = dto.Brand.Trim();
        vehicle.Model         = dto.Model.Trim();
        vehicle.Year          = dto.Year;
        vehicle.FuelType      = dto.FuelType?.Trim();

        var updatedVehicle = await _customerRepository.UpdateVehicleAsync(vehicle);

        return ApiResponse<VehicleResponseDto>.Ok(
            MapToVehicleResponse(updatedVehicle), "Vehicle updated successfully.");
    }

    public async Task<ApiResponse<object>> DeleteVehicleAsync(int customerId, int vehicleId)
    {
        var vehicle = await _customerRepository.GetVehicleByIdAndCustomerIdAsync(vehicleId, customerId);

        if (vehicle == null)
            return ApiResponse<object>.Fail("Vehicle not found.");

        var deleted = await _customerRepository.DeleteVehicleAsync(vehicle);

        if (!deleted)
            return ApiResponse<object>.Fail("Vehicle could not be deleted.");

        return ApiResponse<object>.Ok(null, "Vehicle deleted successfully.");
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private static CustomerResponseDto MapToCustomerResponse(Customer customer) => new()
    {
        Id          = customer.Id,
        FullName    = customer.FullName,
        PhoneNumber = customer.PhoneNumber,
        Email       = customer.Email,
        Address     = customer.Address,
        CreatedAt   = customer.CreatedAt,
        Vehicles    = customer.Vehicles?.Select(MapToVehicleResponse).ToList() ?? new()
    };

    private static VehicleResponseDto MapToVehicleResponse(Vehicle v) => new()
    {
        Id            = v.Id,
        VehicleNumber = v.VehicleNumber,
        Brand         = v.Brand,
        Model         = v.Model,
        Year          = v.Year,
        FuelType      = v.FuelType
    };

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    private static string BuildInvoiceNumber(int invoiceId) =>
        $"INV-{invoiceId:D6}";
}
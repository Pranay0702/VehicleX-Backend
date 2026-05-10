using System.Net;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<CustomerResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var customers = await _customerRepository.GetAllAsync(cancellationToken);
            return ServiceResult<IReadOnlyList<CustomerResponse>>.Ok(
                customers.Select(ToResponse).ToList(),
                "Customers retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<CustomerResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<CustomerResponse>.Fail("Invalid customer id.", (int)HttpStatusCode.BadRequest);
            }

            var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<CustomerResponse>.Fail("Customer was not found.", (int)HttpStatusCode.NotFound);
            }

            return ServiceResult<CustomerResponse>.Ok(ToResponse(customer), "Customer retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<CustomerResponse>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return ServiceResult<CustomerResponse>.Fail("Customer request is required.", (int)HttpStatusCode.BadRequest);
            }

            var email = NormalizeEmail(request.Email);
            if (await _customerRepository.EmailExistsAsync(email, null, cancellationToken))
            {
                return ServiceResult<CustomerResponse>.Fail("A customer with this email already exists.", (int)HttpStatusCode.Conflict);
            }

            var customer = new Customer
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PhoneNumber = request.PhoneNumber.Trim(),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _customerRepository.AddAsync(customer, cancellationToken);
            await _customerRepository.SaveChangesAsync(cancellationToken);

            return ServiceResult<CustomerResponse>.Ok(ToResponse(customer), "Customer registered successfully.", (int)HttpStatusCode.Created);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static CustomerResponse ToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            FullName = $"{customer.FirstName} {customer.LastName}".Trim(),
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            IsActive = customer.IsActive,
            CreatedAtUtc = customer.CreatedAtUtc,
            UpdatedAtUtc = customer.UpdatedAtUtc
        };
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}

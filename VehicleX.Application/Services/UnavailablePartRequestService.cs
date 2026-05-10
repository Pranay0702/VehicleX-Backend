using System.Net;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Domain.Enums;

namespace VehicleX.Application.Services;

public class UnavailablePartRequestService : IUnavailablePartRequestService
{
    private readonly IUnavailablePartRequestRepository _partRequestRepository;
    private readonly ICustomerRepository _customerRepository;

    public UnavailablePartRequestService(
        IUnavailablePartRequestRepository partRequestRepository,
        ICustomerRepository customerRepository)
    {
        _partRequestRepository = partRequestRepository;
        _customerRepository = customerRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<UnavailablePartRequestResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var partRequests = await _partRequestRepository.GetAllAsync(cancellationToken);
            return ServiceResult<IReadOnlyList<UnavailablePartRequestResponse>>.Ok(
                partRequests.Select(ToResponse).ToList(),
                "Unavailable part requests retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<IReadOnlyList<UnavailablePartRequestResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        try
        {
            if (customerId <= 0)
            {
                return ServiceResult<IReadOnlyList<UnavailablePartRequestResponse>>.Fail("Invalid customer id.", (int)HttpStatusCode.BadRequest);
            }

            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<IReadOnlyList<UnavailablePartRequestResponse>>.Fail("Customer was not found.", (int)HttpStatusCode.NotFound);
            }

            var partRequests = await _partRequestRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            return ServiceResult<IReadOnlyList<UnavailablePartRequestResponse>>.Ok(
                partRequests.Select(ToResponse).ToList(),
                "Customer unavailable part requests retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<UnavailablePartRequestResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<UnavailablePartRequestResponse>.Fail("Invalid unavailable part request id.", (int)HttpStatusCode.BadRequest);
            }

            var partRequest = await _partRequestRepository.GetByIdAsync(id, cancellationToken);
            if (partRequest is null)
            {
                return ServiceResult<UnavailablePartRequestResponse>.Fail("Unavailable part request was not found.", (int)HttpStatusCode.NotFound);
            }

            return ServiceResult<UnavailablePartRequestResponse>.Ok(ToResponse(partRequest), "Unavailable part request retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<UnavailablePartRequestResponse>> CreateAsync(RequestUnavailablePartRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return ServiceResult<UnavailablePartRequestResponse>.Fail("Unavailable part request is required.", (int)HttpStatusCode.BadRequest);
            }

            var validationErrors = ValidateRequest(request);
            if (validationErrors.Count > 0)
            {
                return ServiceResult<UnavailablePartRequestResponse>.Fail("Validation failed.", (int)HttpStatusCode.BadRequest, validationErrors);
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<UnavailablePartRequestResponse>.Fail("Customer was not found.", (int)HttpStatusCode.NotFound);
            }

            var partRequest = new UnavailablePartRequest
            {
                CustomerId = request.CustomerId,
                PartName = request.PartName.Trim(),
                PartNumber = NormalizeOptional(request.PartNumber),
                VehicleMake = NormalizeOptional(request.VehicleMake),
                VehicleModel = NormalizeOptional(request.VehicleModel),
                Quantity = request.Quantity,
                Notes = NormalizeOptional(request.Notes),
                Status = PartRequestStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow,
                Customer = customer
            };

            await _partRequestRepository.AddAsync(partRequest, cancellationToken);
            await _partRequestRepository.SaveChangesAsync(cancellationToken);

            return ServiceResult<UnavailablePartRequestResponse>.Ok(
                ToResponse(partRequest),
                "Unavailable part request submitted successfully.",
                (int)HttpStatusCode.Created);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static UnavailablePartRequestResponse ToResponse(UnavailablePartRequest partRequest)
    {
        var customerName = partRequest.Customer is null
            ? string.Empty
            : $"{partRequest.Customer.FirstName} {partRequest.Customer.LastName}".Trim();

        return new UnavailablePartRequestResponse
        {
            Id = partRequest.Id,
            CustomerId = partRequest.CustomerId,
            CustomerName = customerName,
            PartName = partRequest.PartName,
            PartNumber = partRequest.PartNumber,
            VehicleMake = partRequest.VehicleMake,
            VehicleModel = partRequest.VehicleModel,
            Quantity = partRequest.Quantity,
            Notes = partRequest.Notes,
            Status = partRequest.Status,
            StatusName = partRequest.Status.ToString(),
            CreatedAtUtc = partRequest.CreatedAtUtc,
            UpdatedAtUtc = partRequest.UpdatedAtUtc
        };
    }

    private static Dictionary<string, string[]> ValidateRequest(RequestUnavailablePartRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.CustomerId <= 0)
        {
            errors["customerId"] = ["CustomerId must be greater than zero."];
        }

        if (request.Quantity <= 0)
        {
            errors["quantity"] = ["Quantity must be greater than zero."];
        }

        return errors;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

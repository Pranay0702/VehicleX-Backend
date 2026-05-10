using System.Net;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Domain.Enums;

namespace VehicleX.Application.Services;

public class ServiceReviewService : IServiceReviewService
{
    private readonly IServiceReviewRepository _serviceReviewRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public ServiceReviewService(
        IServiceReviewRepository serviceReviewRepository,
        ICustomerRepository customerRepository,
        IAppointmentRepository appointmentRepository)
    {
        _serviceReviewRepository = serviceReviewRepository;
        _customerRepository = customerRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<ServiceReviewResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _serviceReviewRepository.GetAllAsync(cancellationToken);
            return ServiceResult<IReadOnlyList<ServiceReviewResponse>>.Ok(
                reviews.Select(ToResponse).ToList(),
                "Service reviews retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<IReadOnlyList<ServiceReviewResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        try
        {
            if (customerId <= 0)
            {
                return ServiceResult<IReadOnlyList<ServiceReviewResponse>>.Fail("Invalid customer id.", (int)HttpStatusCode.BadRequest);
            }

            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<IReadOnlyList<ServiceReviewResponse>>.Fail("Customer was not found.", (int)HttpStatusCode.NotFound);
            }

            var reviews = await _serviceReviewRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            return ServiceResult<IReadOnlyList<ServiceReviewResponse>>.Ok(
                reviews.Select(ToResponse).ToList(),
                "Customer service reviews retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<ServiceReviewResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<ServiceReviewResponse>.Fail("Invalid service review id.", (int)HttpStatusCode.BadRequest);
            }

            var review = await _serviceReviewRepository.GetByIdAsync(id, cancellationToken);
            if (review is null)
            {
                return ServiceResult<ServiceReviewResponse>.Fail("Service review was not found.", (int)HttpStatusCode.NotFound);
            }

            return ServiceResult<ServiceReviewResponse>.Ok(ToResponse(review), "Service review retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<ServiceReviewResponse>> CreateAsync(CreateServiceReviewRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return ServiceResult<ServiceReviewResponse>.Fail("Service review request is required.", (int)HttpStatusCode.BadRequest);
            }

            var validationErrors = ValidateRequest(request);
            if (validationErrors.Count > 0)
            {
                return ServiceResult<ServiceReviewResponse>.Fail("Validation failed.", (int)HttpStatusCode.BadRequest, validationErrors);
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<ServiceReviewResponse>.Fail("Customer was not found.", (int)HttpStatusCode.NotFound);
            }

            Appointment? appointment = null;
            if (request.AppointmentId.HasValue)
            {
                appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId.Value, cancellationToken);
                if (appointment is null)
                {
                    return ServiceResult<ServiceReviewResponse>.Fail("Appointment was not found.", (int)HttpStatusCode.NotFound);
                }

                if (appointment.CustomerId != request.CustomerId)
                {
                    return ServiceResult<ServiceReviewResponse>.Fail("Appointment does not belong to the selected customer.", (int)HttpStatusCode.BadRequest);
                }

                if (await _serviceReviewRepository.AppointmentReviewExistsAsync(request.AppointmentId.Value, cancellationToken))
                {
                    return ServiceResult<ServiceReviewResponse>.Fail("This appointment has already been reviewed.", (int)HttpStatusCode.Conflict);
                }
            }

            var review = new ServiceReview
            {
                CustomerId = request.CustomerId,
                AppointmentId = request.AppointmentId,
                Rating = request.Rating,
                Comment = request.Comment.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                Customer = customer,
                Appointment = appointment
            };

            await _serviceReviewRepository.AddAsync(review, cancellationToken);
            await _serviceReviewRepository.SaveChangesAsync(cancellationToken);

            return ServiceResult<ServiceReviewResponse>.Ok(ToResponse(review), "Service review submitted successfully.", (int)HttpStatusCode.Created);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static ServiceReviewResponse ToResponse(ServiceReview review)
    {
        var customerName = review.Customer is null
            ? string.Empty
            : $"{review.Customer.FirstName} {review.Customer.LastName}".Trim();

        return new ServiceReviewResponse
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            CustomerName = customerName,
            AppointmentId = review.AppointmentId,
            Rating = review.Rating,
            RatingName = review.Rating.ToString(),
            Comment = review.Comment,
            CreatedAtUtc = review.CreatedAtUtc,
            UpdatedAtUtc = review.UpdatedAtUtc
        };
    }

    private static Dictionary<string, string[]> ValidateRequest(CreateServiceReviewRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.CustomerId <= 0)
        {
            errors["customerId"] = ["CustomerId must be greater than zero."];
        }

        if (request.AppointmentId.HasValue && request.AppointmentId.Value <= 0)
        {
            errors["appointmentId"] = ["AppointmentId must be greater than zero."];
        }

        if (!Enum.IsDefined(typeof(ServiceRating), request.Rating))
        {
            errors["rating"] = ["Rating must be between one and five."];
        }

        return errors;
    }
}

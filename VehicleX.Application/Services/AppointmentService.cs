using System.Net;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Domain.Enums;

namespace VehicleX.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICustomerRepository _customerRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository, ICustomerRepository customerRepository)
    {
        _appointmentRepository = appointmentRepository;
        _customerRepository = customerRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<AppointmentResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            return ServiceResult<IReadOnlyList<AppointmentResponse>>.Ok(
                appointments.Select(ToResponse).ToList(),
                "Appointments retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<IReadOnlyList<AppointmentResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        try
        {
            if (customerId <= 0)
            {
                return ServiceResult<IReadOnlyList<AppointmentResponse>>.Fail("Invalid customer id.", (int)HttpStatusCode.BadRequest);
            }

            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<IReadOnlyList<AppointmentResponse>>.Fail("Customer was not found.", (int)HttpStatusCode.NotFound);
            }

            var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            return ServiceResult<IReadOnlyList<AppointmentResponse>>.Ok(
                appointments.Select(ToResponse).ToList(),
                "Customer appointments retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<AppointmentResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<AppointmentResponse>.Fail("Invalid appointment id.", (int)HttpStatusCode.BadRequest);
            }

            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null)
            {
                return ServiceResult<AppointmentResponse>.Fail("Appointment was not found.", (int)HttpStatusCode.NotFound);
            }

            return ServiceResult<AppointmentResponse>.Ok(ToResponse(appointment), "Appointment retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<AppointmentResponse>> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
            {
                return ServiceResult<AppointmentResponse>.Fail("Appointment request is required.", (int)HttpStatusCode.BadRequest);
            }

            var validationErrors = ValidateBooking(request);
            if (validationErrors.Count > 0)
            {
                return ServiceResult<AppointmentResponse>.Fail("Validation failed.", (int)HttpStatusCode.BadRequest, validationErrors);
            }

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return ServiceResult<AppointmentResponse>.Fail("Customer was not found.", (int)HttpStatusCode.NotFound);
            }

            var appointment = new Appointment
            {
                CustomerId = request.CustomerId,
                AppointmentDateUtc = request.AppointmentDateUtc,
                ServiceType = request.ServiceType.Trim(),
                VehicleMake = NormalizeOptional(request.VehicleMake),
                VehicleModel = NormalizeOptional(request.VehicleModel),
                VehicleRegistrationNumber = NormalizeOptional(request.VehicleRegistrationNumber),
                Notes = NormalizeOptional(request.Notes),
                Status = AppointmentStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow,
                Customer = customer
            };

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            return ServiceResult<AppointmentResponse>.Ok(ToResponse(appointment), "Appointment booked successfully.", (int)HttpStatusCode.Created);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static AppointmentResponse ToResponse(Appointment appointment)
    {
        var customerName = appointment.Customer is null
            ? string.Empty
            : $"{appointment.Customer.FirstName} {appointment.Customer.LastName}".Trim();

        return new AppointmentResponse
        {
            Id = appointment.Id,
            CustomerId = appointment.CustomerId,
            CustomerName = customerName,
            AppointmentDateUtc = appointment.AppointmentDateUtc,
            ServiceType = appointment.ServiceType,
            VehicleMake = appointment.VehicleMake,
            VehicleModel = appointment.VehicleModel,
            VehicleRegistrationNumber = appointment.VehicleRegistrationNumber,
            Notes = appointment.Notes,
            Status = appointment.Status,
            StatusName = appointment.Status.ToString(),
            CreatedAtUtc = appointment.CreatedAtUtc,
            UpdatedAtUtc = appointment.UpdatedAtUtc
        };
    }

    private static Dictionary<string, string[]> ValidateBooking(BookAppointmentRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.CustomerId <= 0)
        {
            errors["customerId"] = ["CustomerId must be greater than zero."];
        }

        if (request.AppointmentDateUtc <= DateTime.UtcNow)
        {
            errors["appointmentDateUtc"] = ["Appointment date must be in the future."];
        }

        return errors;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

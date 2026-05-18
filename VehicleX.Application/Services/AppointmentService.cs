using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Appointments;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
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
        _customerRepository    = customerRepository;
    }

    public async Task<ApiResponse<IReadOnlyList<AppointmentResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            return ApiResponse<IReadOnlyList<AppointmentResponse>>.Ok(
                appointments.Select(ToResponse).ToList(),
                "Appointments retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ApiResponse<IReadOnlyList<AppointmentResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        try
        {
            if (customerId <= 0)
                return ApiResponse<IReadOnlyList<AppointmentResponse>>.Fail("Invalid customer id.");

            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer is null)
                return ApiResponse<IReadOnlyList<AppointmentResponse>>.Fail("Customer was not found.");

            var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            return ApiResponse<IReadOnlyList<AppointmentResponse>>.Ok(
                appointments.Select(ToResponse).ToList(),
                "Customer appointments retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ApiResponse<AppointmentResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
                return ApiResponse<AppointmentResponse>.Fail("Invalid appointment id.");

            var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
            if (appointment is null)
                return ApiResponse<AppointmentResponse>.Fail("Appointment was not found.");

            return ApiResponse<AppointmentResponse>.Ok(ToResponse(appointment), "Appointment retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ApiResponse<AppointmentResponse>> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request is null)
                return ApiResponse<AppointmentResponse>.Fail("Appointment request is required.");

            var validationErrors = ValidateBooking(request);
            if (validationErrors.Count > 0)
                return ApiResponse<AppointmentResponse>.Fail("Validation failed.", validationErrors);

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
                return ApiResponse<AppointmentResponse>.Fail("Customer was not found.");

            var appointment = new Appointment
            {
                CustomerId                 = request.CustomerId,
                AppointmentDateUtc         = request.AppointmentDateUtc,
                ServiceType                = request.ServiceType.Trim(),
                VehicleMake                = NormalizeOptional(request.VehicleMake),
                VehicleModel               = NormalizeOptional(request.VehicleModel),
                VehicleRegistrationNumber  = NormalizeOptional(request.VehicleRegistrationNumber),
                Notes                      = NormalizeOptional(request.Notes),
                Status                     = AppointmentStatus.Pending,
                CreatedAtUtc               = DateTime.UtcNow,
                Customer                   = customer
            };

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<AppointmentResponse>.Ok(ToResponse(appointment), "Appointment booked successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static AppointmentResponse ToResponse(Appointment appointment)
    {
        return new AppointmentResponse
        {
            Id                         = appointment.Id,
            CustomerId                 = appointment.CustomerId,
            CustomerName               = appointment.Customer?.FullName ?? string.Empty,
            AppointmentDateUtc         = appointment.AppointmentDateUtc,
            ServiceType                = appointment.ServiceType,
            VehicleMake                = appointment.VehicleMake,
            VehicleModel               = appointment.VehicleModel,
            VehicleRegistrationNumber  = appointment.VehicleRegistrationNumber,
            Notes                      = appointment.Notes,
            Status                     = appointment.Status,
            StatusName                 = appointment.Status.ToString(),
            CreatedAtUtc               = appointment.CreatedAtUtc,
            UpdatedAtUtc               = appointment.UpdatedAtUtc
        };
    }

    private static Dictionary<string, string[]> ValidateBooking(BookAppointmentRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.CustomerId <= 0)
            errors["customerId"] = ["CustomerId must be greater than zero."];

        if (request.AppointmentDateUtc <= DateTime.UtcNow)
            errors["appointmentDateUtc"] = ["Appointment date must be in the future."];

        return errors;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Appointments;

namespace VehicleX.Application.Interfaces.Services;

public interface IAppointmentService
{
    Task<ApiResponse<IReadOnlyList<AppointmentResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<IReadOnlyList<AppointmentResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<ApiResponse<AppointmentResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ApiResponse<AppointmentResponse>> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken);
}

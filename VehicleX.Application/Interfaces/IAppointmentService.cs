using VehicleX.Application.Common;
using VehicleX.Application.DTOs;

namespace VehicleX.Application.Interfaces;

public interface IAppointmentService
{
    Task<ServiceResult<IReadOnlyList<AppointmentResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<AppointmentResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<ServiceResult<AppointmentResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ServiceResult<AppointmentResponse>> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken);
}

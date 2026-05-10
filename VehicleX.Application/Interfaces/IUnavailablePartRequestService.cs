using VehicleX.Application.Common;
using VehicleX.Application.DTOs;

namespace VehicleX.Application.Interfaces;

public interface IUnavailablePartRequestService
{
    Task<ServiceResult<IReadOnlyList<UnavailablePartRequestResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<UnavailablePartRequestResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<ServiceResult<UnavailablePartRequestResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ServiceResult<UnavailablePartRequestResponse>> CreateAsync(RequestUnavailablePartRequest request, CancellationToken cancellationToken);
}

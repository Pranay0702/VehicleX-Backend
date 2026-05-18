using VehicleX.Application.Common;
using VehicleX.Application.DTOs.UnavailablePartRequests;

namespace VehicleX.Application.Interfaces.Services;

public interface IUnavailablePartRequestService
{
    Task<ApiResponse<IReadOnlyList<UnavailablePartRequestResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<IReadOnlyList<UnavailablePartRequestResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<ApiResponse<UnavailablePartRequestResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ApiResponse<UnavailablePartRequestResponse>> CreateAsync(RequestUnavailablePartRequest request, CancellationToken cancellationToken);
}

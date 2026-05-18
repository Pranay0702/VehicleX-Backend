using VehicleX.Application.Common;
using VehicleX.Application.DTOs.ServiceReviews;

namespace VehicleX.Application.Interfaces.Services;

public interface IServiceReviewService
{
    Task<ApiResponse<IReadOnlyList<ServiceReviewResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<IReadOnlyList<ServiceReviewResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<ApiResponse<ServiceReviewResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ApiResponse<ServiceReviewResponse>> CreateAsync(CreateServiceReviewRequest request, CancellationToken cancellationToken);
}

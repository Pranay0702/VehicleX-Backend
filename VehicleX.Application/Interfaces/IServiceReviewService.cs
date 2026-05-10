using VehicleX.Application.Common;
using VehicleX.Application.DTOs;

namespace VehicleX.Application.Interfaces;

public interface IServiceReviewService
{
    Task<ServiceResult<IReadOnlyList<ServiceReviewResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<ServiceReviewResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<ServiceResult<ServiceReviewResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ServiceResult<ServiceReviewResponse>> CreateAsync(CreateServiceReviewRequest request, CancellationToken cancellationToken);
}

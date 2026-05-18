using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Shared;

namespace VehicleX.Application.Interfaces.Services;

public interface ICustomerHistoryService
{
    Task<ApiResponse<CustomerHistoryResponse>> GetHistoryAsync(int customerId, CancellationToken cancellationToken);
    Task<ApiResponse<IReadOnlyList<CustomerPurchaseHistoryResponse>>> GetPurchaseHistoryAsync(int customerId, CancellationToken cancellationToken);
    Task<ApiResponse<IReadOnlyList<CustomerServiceHistoryResponse>>> GetServiceHistoryAsync(int customerId, CancellationToken cancellationToken);
}

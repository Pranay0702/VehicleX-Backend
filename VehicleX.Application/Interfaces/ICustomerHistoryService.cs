using VehicleX.Application.Common;
using VehicleX.Application.DTOs;

namespace VehicleX.Application.Interfaces;

public interface ICustomerHistoryService
{
    Task<ServiceResult<CustomerHistoryResponse>> GetHistoryAsync(int customerId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CustomerPurchaseHistoryResponse>>> GetPurchaseHistoryAsync(int customerId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CustomerServiceHistoryResponse>>> GetServiceHistoryAsync(int customerId, CancellationToken cancellationToken);
}

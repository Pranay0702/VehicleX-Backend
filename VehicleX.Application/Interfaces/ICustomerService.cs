using VehicleX.Application.Common;
using VehicleX.Application.DTOs;

namespace VehicleX.Application.Interfaces;

public interface ICustomerService
{
    Task<ServiceResult<IReadOnlyList<CustomerResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ServiceResult<CustomerResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ServiceResult<CustomerResponse>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken);
}

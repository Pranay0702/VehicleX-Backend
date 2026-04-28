using VehicleX.Application.Common;
using VehicleX.Application.DTOs;

namespace VehicleX.Application.Interfaces;

public interface IStaffService
{
    Task<ServiceResult<IReadOnlyList<StaffResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ServiceResult<StaffResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ServiceResult<StaffResponse>> CreateAsync(CreateStaffRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<StaffResponse>> UpdateAsync(int id, UpdateStaffRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<StaffResponse>> UpdateRoleAsync(int id, UpdateStaffRoleRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken cancellationToken);
}

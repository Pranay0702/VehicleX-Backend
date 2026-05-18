using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Staff;

namespace VehicleX.Application.Interfaces.Services;

public interface IStaffService
{
    Task<ApiResponse<IReadOnlyList<StaffResponse>>> GetAllAsync(CancellationToken cancellationToken);
    Task<ApiResponse<StaffResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ApiResponse<StaffResponse>> CreateAsync(CreateStaffRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<StaffResponse>> UpdateAsync(int id, UpdateStaffRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<StaffResponse>> UpdateRoleAsync(int id, UpdateStaffRoleRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<object>> DeleteAsync(int id, CancellationToken cancellationToken);
}

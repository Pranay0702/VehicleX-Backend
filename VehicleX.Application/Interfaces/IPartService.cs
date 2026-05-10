using VehicleX.Application.DTOs.Common;
using VehicleX.Application.DTOs.Part;

namespace VehicleX.Application.Interfaces;

public interface IPartService
{
    Task<ApiResponse<IEnumerable<PartResponseDto>>> GetAllPartsAsync();

    Task<ApiResponse<PartResponseDto>> GetPartByIdAsync(int id);
    
    Task<ApiResponse<PartResponseDto>> CreatePartAsync(CreatePartDto dto);

    Task<ApiResponse<PartResponseDto>> UpdatePartAsync(int id, UpdatePartDto dto);
    
    Task<ApiResponse<bool>> DeletePartAsync(int id);
}
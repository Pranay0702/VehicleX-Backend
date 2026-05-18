using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Vendor;

namespace VehicleX.Application.Interfaces.Services;

public interface IVendorService
{
    Task<ApiResponse<IEnumerable<VendorResponseDto>>> GetAllVendorsAsync();

    Task<ApiResponse<VendorResponseDto>> GetVendorByIdAsync(int id);
    
    Task<ApiResponse<VendorResponseDto>> CreateVendorAsync(CreateVendorDto dto);

    Task<ApiResponse<VendorResponseDto>> UpdateVendorAsync(int id, UpdateVendorDto dto);

    Task<ApiResponse<bool>> DeleteVendorAsync(int id);
}
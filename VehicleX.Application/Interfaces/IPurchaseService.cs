using VehicleX.Application.DTOs.Common;
using VehicleX.Application.DTOs.Purchase;

namespace VehicleX.Application.Interfaces;

public interface IPurchaseService
{

    Task<ApiResponse<IEnumerable<PurchaseResponseDto>>> GetAllPurchasesAsync();

    Task<ApiResponse<PurchaseResponseDto>> GetPurchaseByIdAsync(int id);
    
    Task<ApiResponse<PurchaseResponseDto>> CreatePurchaseAsync(CreatePurchaseDto dto);
}
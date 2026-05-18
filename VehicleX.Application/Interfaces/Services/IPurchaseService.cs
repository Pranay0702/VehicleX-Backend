using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Purchase;

namespace VehicleX.Application.Interfaces.Services;

public interface IPurchaseService
{

    Task<ApiResponse<IEnumerable<PurchaseResponseDto>>> GetAllPurchasesAsync();

    Task<ApiResponse<PurchaseResponseDto>> GetPurchaseByIdAsync(int id);
    
    Task<ApiResponse<PurchaseResponseDto>> CreatePurchaseAsync(CreatePurchaseDto dto);
}
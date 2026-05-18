using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Customers;

namespace VehicleX.Application.Interfaces.Services;

public interface ICustomerService
{
    // Registration & Auth
    Task<ApiResponse<CustomerResponseDto>> StaffRegisterCustomerAsync(StaffRegisterCustomerDto dto);

    Task<ApiResponse<CustomerAuthResponseDto>> CustomerSelfRegisterAsync(CustomerSelfRegisterDto dto);

    Task<ApiResponse<CustomerAuthResponseDto>> CustomerLoginAsync(CustomerLoginDto dto);

    // Staff-facing queries
    Task<ApiResponse<List<CustomerResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerResponseDto>> GetByIdAsync(int customerId, CancellationToken cancellationToken = default);

    Task<ApiResponse<List<CustomerSearchResultDto>>> SearchCustomersAsync(string searchTerm);

    Task<ApiResponse<CustomerDetailsHistoryResponseDto>> GetCustomerDetailsAndHistoryForStaffAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    // Customer self-service
    Task<ApiResponse<CustomerResponseDto>> GetProfileAsync(int customerId);

    Task<ApiResponse<CustomerResponseDto>> UpdateProfileAsync(int customerId, UpdateCustomerProfileDto dto);

    Task<ApiResponse<List<VehicleResponseDto>>> GetMyVehiclesAsync(int customerId);

    Task<ApiResponse<VehicleResponseDto>> AddVehicleAsync(int customerId, CreateCustomerVehicleDto dto);

    Task<ApiResponse<VehicleResponseDto>> UpdateVehicleAsync(int customerId, int vehicleId, UpdateCustomerVehicleDto dto);

    Task<ApiResponse<object>> DeleteVehicleAsync(int customerId, int vehicleId);
}
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Customers;

namespace VehicleX.Application.Interfaces.Services;

public interface ICustomerService
{
    // will accept customer registration data, save it, and return a clean API response.
    // Task means it works asynchronously
    Task<ApiResponse<CustomerResponseDto>> StaffRegisterCustomerAsync(StaffRegisterCustomerDto dto);

    // Customer can register by themselves and receive JWT token
    Task<ApiResponse<CustomerAuthResponseDto>> CustomerSelfRegisterAsync(CustomerSelfRegisterDto dto);
}
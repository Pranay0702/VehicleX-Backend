using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Customers;

namespace VehicleX.Application.Interfaces.Services;

public interface ICustomerService
{
    // will accept customer registration data, save it, and return a clean API response.
    // Task means it works asynchronously
    Task<ApiResponse<CustomerResponseDto>> StaffRegisterCustomerAsync(StaffRegisterCustomerDto dto);
}
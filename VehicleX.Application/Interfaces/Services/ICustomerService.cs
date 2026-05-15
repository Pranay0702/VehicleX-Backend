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

    // Service will check password + generate JWT.
    Task<ApiResponse<CustomerAuthResponseDto>> CustomerLoginAsync(CustomerLoginDto dto);

    // Service will search for customers based on the search term and return a list of matching customers.
    Task<ApiResponse<List<CustomerSearchResultDto>>> SearchCustomersAsync(string searchTerm);

    // Service will retrieve the profile information of a specific customer based on their ID and return it in a clean API response
    Task<ApiResponse<CustomerResponseDto>> GetProfileAsync(int customerId);

    // Service will update the profile information of a specific customer based on their ID and the provided update data, and return the updated profile in a clean API response
    Task<ApiResponse<CustomerResponseDto>> UpdateProfileAsync(int customerId, UpdateCustomerProfileDto dto);

    // Service will retrieve a list of vehicles associated with a specific customer based on their ID and return it in a clean API response
    Task<ApiResponse<List<VehicleResponseDto>>> GetMyVehiclesAsync(int customerId);

    // Service will add a new vehicle to the customer's profile based on their ID and the provided vehicle data, and return the added vehicle in a clean API response
    Task<ApiResponse<VehicleResponseDto>> AddVehicleAsync(int customerId, CreateCustomerVehicleDto dto);

    // Service will update the information of a specific vehicle associated with the customer's profile based on the customer ID, vehicle ID, and the provided update data, and return the updated vehicle in a clean API responsw
    Task<ApiResponse<VehicleResponseDto>> UpdateVehicleAsync(int customerId, int vehicleId, UpdateCustomerVehicleDto dto);

    // Service will delete a specific vehicle from the customer's profile based on the customer ID and vehicle ID, and return a clean API response indicating the success or failure of the operation
    Task<ApiResponse<object>> DeleteVehicleAsync(int customerId, int vehicleId);
}
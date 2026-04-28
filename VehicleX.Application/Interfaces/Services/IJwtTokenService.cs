using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Services;

public interface IJwtTokenService
{
    // Creates JWT token after customer registration/login
    string GenerateCustomerToken(Customer customer);
}
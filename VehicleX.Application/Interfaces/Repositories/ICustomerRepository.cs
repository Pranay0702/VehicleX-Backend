using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdWithVehiclesAsync(int customerId);

    Task<Customer?> GetByEmailAsync(string email);

    Task<Customer> AddAsync(Customer customer);

    Task<Customer> UpdateAsync(Customer customer);

    Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    
    Task<bool> EmailExistsAsync(string email, int? excludedCustomerId = null);

    Task<bool> VehicleNumberExistsAsync(string vehicleNumber);

    Task<bool> VehicleNumberExistsForOtherCustomerAsync(string vehicleNumber, int customerId, int? vehicleId = null);

    Task<List<Customer>> SearchCustomersAsync(string searchTerm);

    Task<List<Vehicle>> GetVehiclesByCustomerIdAsync(int customerId);

    Task<Vehicle?> GetVehicleByIdAndCustomerIdAsync(int vehicleId, int customerId);

    Task<Vehicle> AddVehicleAsync(Vehicle vehicle);

    Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle);

    Task<bool> DeleteVehicleAsync(Vehicle vehicle);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
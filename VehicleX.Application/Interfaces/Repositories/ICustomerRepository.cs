using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken = default);

    Task<Customer> AddAsync(Customer customer);

    Task<bool> PhoneNumberExistsAsync(string phoneNumber);

    Task<bool> EmailExistsAsync(string email);

    Task<Customer?> GetByEmailAsync(string email);

    Task<List<Customer>> SearchCustomersAsync(string searchTerm);
}
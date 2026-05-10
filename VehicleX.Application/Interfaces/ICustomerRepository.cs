using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(string email, int? excludedCustomerId, CancellationToken cancellationToken);
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    void Update(Customer customer);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

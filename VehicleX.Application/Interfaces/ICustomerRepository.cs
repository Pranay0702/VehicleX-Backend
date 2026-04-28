using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken = default);
}

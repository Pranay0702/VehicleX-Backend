using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface ICustomerPurchaseRepository
{
    Task<IReadOnlyList<CustomerPurchase>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
}

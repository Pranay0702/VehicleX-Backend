using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface ICustomerPurchaseRepository
{
    Task<IReadOnlyList<CustomerPurchase>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
}

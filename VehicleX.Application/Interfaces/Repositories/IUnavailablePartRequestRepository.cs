using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface IUnavailablePartRequestRepository
{
    Task<IReadOnlyList<UnavailablePartRequest>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<UnavailablePartRequest>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<UnavailablePartRequest?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(UnavailablePartRequest partRequest, CancellationToken cancellationToken);
    void Update(UnavailablePartRequest partRequest);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

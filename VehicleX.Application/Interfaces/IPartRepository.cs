using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface IPartRepository
{
    Task<List<Part>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<Part>> GetByIdsAsync(IReadOnlyCollection<int> partIds, CancellationToken cancellationToken = default);
}

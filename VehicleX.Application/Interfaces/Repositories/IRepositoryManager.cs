namespace VehicleX.Application.Interfaces.Repositories;

public interface IRepositoryManager
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

namespace VehicleX.Application.Interfaces;

public interface IRepositoryManager
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

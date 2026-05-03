using VehicleX.Application.Interfaces;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryManager(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

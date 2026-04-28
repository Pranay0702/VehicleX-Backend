using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class PartRepository : IPartRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PartRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Part>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Parts
            .AsNoTracking()
            .OrderBy(part => part.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Part>> GetByIdsAsync(IReadOnlyCollection<int> partIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Parts
            .Where(part => partIds.Contains(part.Id))
            .ToListAsync(cancellationToken);
    }
}

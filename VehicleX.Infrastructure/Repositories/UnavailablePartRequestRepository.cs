using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class UnavailablePartRequestRepository : IUnavailablePartRequestRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UnavailablePartRequestRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UnavailablePartRequest>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.UnavailablePartRequests
            .AsNoTracking()
            .Include(partRequest => partRequest.Customer)
            .OrderByDescending(partRequest => partRequest.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnavailablePartRequest>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.UnavailablePartRequests
            .AsNoTracking()
            .Include(partRequest => partRequest.Customer)
            .Where(partRequest => partRequest.CustomerId == customerId)
            .OrderByDescending(partRequest => partRequest.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<UnavailablePartRequest?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.UnavailablePartRequests
            .Include(partRequest => partRequest.Customer)
            .FirstOrDefaultAsync(partRequest => partRequest.Id == id, cancellationToken);
    }

    public async Task AddAsync(UnavailablePartRequest partRequest, CancellationToken cancellationToken)
    {
        await _dbContext.UnavailablePartRequests.AddAsync(partRequest, cancellationToken);
    }

    public void Update(UnavailablePartRequest partRequest)
    {
        _dbContext.UnavailablePartRequests.Update(partRequest);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

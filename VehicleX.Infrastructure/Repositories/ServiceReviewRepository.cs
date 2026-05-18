using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class ServiceReviewRepository : IServiceReviewRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ServiceReviewRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ServiceReview>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceReviews
            .AsNoTracking()
            .Include(review => review.Customer)
            .Include(review => review.Appointment)
            .OrderByDescending(review => review.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceReview>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceReviews
            .AsNoTracking()
            .Include(review => review.Customer)
            .Include(review => review.Appointment)
            .Where(review => review.CustomerId == customerId)
            .OrderByDescending(review => review.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceReview?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceReviews
            .Include(review => review.Customer)
            .Include(review => review.Appointment)
            .FirstOrDefaultAsync(review => review.Id == id, cancellationToken);
    }

    public async Task<bool> AppointmentReviewExistsAsync(int appointmentId, CancellationToken cancellationToken)
    {
        return await _dbContext.ServiceReviews
            .AnyAsync(review => review.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task AddAsync(ServiceReview serviceReview, CancellationToken cancellationToken)
    {
        await _dbContext.ServiceReviews.AddAsync(serviceReview, cancellationToken);
    }

    public void Update(ServiceReview serviceReview)
    {
        _dbContext.ServiceReviews.Update(serviceReview);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

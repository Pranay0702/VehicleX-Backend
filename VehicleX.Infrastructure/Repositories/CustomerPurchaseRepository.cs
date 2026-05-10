using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class CustomerPurchaseRepository : ICustomerPurchaseRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CustomerPurchaseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CustomerPurchase>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.CustomerPurchases
            .AsNoTracking()
            .Include(purchase => purchase.Items)
            .Where(purchase => purchase.CustomerId == customerId)
            .OrderByDescending(purchase => purchase.PurchaseDateUtc)
            .ThenByDescending(purchase => purchase.Id)
            .ToListAsync(cancellationToken);
    }
}

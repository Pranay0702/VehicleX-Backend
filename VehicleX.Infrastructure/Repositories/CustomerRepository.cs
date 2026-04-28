using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CustomerRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);
    }
}

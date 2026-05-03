using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StaffRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Staff>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Staff
            .AsNoTracking()
            .OrderBy(staff => staff.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.Staff
            .FirstOrDefaultAsync(staff => staff.Id == id, cancellationToken);
    }

    public async Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Staff
            .AsNoTracking()
            .FirstOrDefaultAsync(staff => staff.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludedStaffId, CancellationToken cancellationToken)
    {
        return await _dbContext.Staff.AnyAsync(staff =>
            staff.Email == email && (!excludedStaffId.HasValue || staff.Id != excludedStaffId.Value),
            cancellationToken);
    }

    public async Task AddAsync(Staff staff, CancellationToken cancellationToken)
    {
        await _dbContext.Staff.AddAsync(staff, cancellationToken);
    }

    public void Update(Staff staff)
    {
        _dbContext.Staff.Update(staff);
    }

    public void Delete(Staff staff)
    {
        _dbContext.Staff.Remove(staff);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class PartRepository : IPartRepository
{
    private readonly ApplicationDbContext _context;

    public PartRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Part>> GetAllAsync()
    {
        return await _context.Set<Part>()
            .Include(p => p.Vendor)
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Part?> GetByIdAsync(int id)
    {
        return await _context.Set<Part>()
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Part>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var uniqueIds = ids
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (uniqueIds.Count == 0)
        {
            return new List<Part>();
        }

        return await _context.Set<Part>()
            .Where(p => uniqueIds.Contains(p.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByPartNumberAsync(string partNumber, int? excludeId = null)
    {
        var normalized = partNumber.Trim().ToUpper();
        return await _context.Set<Part>()
            .AnyAsync(p => p.PartNumber.ToUpper() == normalized
                           && (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<bool> AnyByVendorIdAsync(int vendorId)
    {
        return await _context.Set<Part>()
            .AnyAsync(p => p.VendorId == vendorId);
    }

    public async Task<Part> AddAsync(Part part)
    {
        await _context.Set<Part>().AddAsync(part);
        await _context.SaveChangesAsync();
        return part;
    }

    public async Task UpdateAsync(Part part)
    {
        _context.Set<Part>().Update(part);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Part part)
    {
        _context.Set<Part>().Remove(part);
        await _context.SaveChangesAsync();
    }
}

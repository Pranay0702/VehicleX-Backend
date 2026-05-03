using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces;
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
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Part?> GetByIdAsync(int id)
    {
        return await _context.Set<Part>()
            .FirstOrDefaultAsync(p => p.Id == id);
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

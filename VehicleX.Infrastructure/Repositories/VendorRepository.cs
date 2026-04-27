using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class VendorRepository : IVendorRepository
{
    private readonly ApplicationDbContext _context;

    public VendorRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Vendor>> GetAllAsync()
    {
        return await _context.Vendors
            .Include(v => v.Parts)
            .OrderByDescending(v => v.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Vendor?> GetByIdAsync(int id)
    {
        return await _context.Vendors
            .Include(v => v.Parts)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
    {
        var normalizedEmail = email.Trim().ToLower();
        return await _context.Vendors
            .AnyAsync(v => v.Email.ToLower() == normalizedEmail
                           && (!excludeId.HasValue || v.Id != excludeId.Value));
    }

    public async Task<Vendor> AddAsync(Vendor vendor)
    {
        await _context.Vendors.AddAsync(vendor);
        await _context.SaveChangesAsync();
        return vendor;
    }

    public async Task UpdateAsync(Vendor vendor)
    {
        _context.Vendors.Update(vendor);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Vendor vendor)
    {
        _context.Vendors.Remove(vendor);
        await _context.SaveChangesAsync();
    }
}
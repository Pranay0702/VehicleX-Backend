using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<PurchaseInvoice>> GetAllAsync()
    {
        return await _context.PurchaseInvoices
            .Include(p => p.Items)
                .ThenInclude(i => i.Part)
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PurchaseInvoice?> GetByIdAsync(int id)
    {
        return await _context.PurchaseInvoices
            .Include(p => p.Items)
                .ThenInclude(i => i.Part)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<PurchaseInvoice> CreatePurchaseAsync(PurchaseInvoice invoice, List<Part> partsToUpdate)
    {
        // Use a transaction to ensure atomicity both the invoice and stock updates succeed or fail together
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // purchase invoice with cascade to items
            await _context.PurchaseInvoices.AddAsync(invoice);

            foreach (var part in partsToUpdate)
            {
                _context.Parts.Update(part);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return invoice;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<int> GetTodaysPurchaseCountAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.PurchaseInvoices
            .CountAsync(p => p.PurchaseDate.Date == today);
    }
}

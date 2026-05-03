using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class PurchaseInvoiceRepository : IPurchaseInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseInvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PurchaseInvoice>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.PurchaseInvoices
            .Where(p => p.InvoiceDate >= from && p.InvoiceDate < to)
            .ToListAsync();
    }
}
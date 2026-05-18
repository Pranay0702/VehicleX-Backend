using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class SalesInvoiceRepository : ISalesInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public SalesInvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SalesInvoice>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.SalesInvoices
            .Where(s => s.InvoiceDate >= from && s.InvoiceDate < to)
            .ToListAsync();
    }

    public async Task<IEnumerable<SalesInvoice>> GetAllWithCustomerAsync()
    {
        return await _context.SalesInvoices
            .Include(s => s.Customer)
            .ToListAsync();
    }

    public async Task<List<SalesInvoice>> GetByCustomerIdWithItemsAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesInvoices
            .AsNoTracking()
            .Include(invoice => invoice.Items)
            .Where(invoice => invoice.CustomerId == customerId)
            .OrderByDescending(invoice => invoice.InvoiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SalesInvoice salesInvoice, CancellationToken cancellationToken = default)
    {
        await _context.SalesInvoices.AddAsync(salesInvoice, cancellationToken);
    }

    public async Task<SalesInvoice?> GetByIdWithItemsAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _context.SalesInvoices
            .Include(invoice => invoice.Customer)
            .Include(invoice => invoice.Items)
            .FirstOrDefaultAsync(invoice => invoice.Id == invoiceId, cancellationToken);
    }
}

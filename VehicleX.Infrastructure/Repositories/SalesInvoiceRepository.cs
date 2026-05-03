using Microsoft.EntityFrameworkCore;
using SalesManagementRepository = VehicleX.Application.Interfaces.ISalesInvoiceRepository;
using SalesReportingRepository = VehicleX.Application.Interfaces.Repositories.ISalesInvoiceRepository;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class SalesInvoiceRepository : SalesManagementRepository, SalesReportingRepository
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

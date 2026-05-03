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
}
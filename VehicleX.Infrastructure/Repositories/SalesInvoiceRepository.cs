using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class SalesInvoiceRepository : ISalesInvoiceRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SalesInvoiceRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(SalesInvoice salesInvoice, CancellationToken cancellationToken = default)
    {
        await _dbContext.SalesInvoices.AddAsync(salesInvoice, cancellationToken);
    }

    public async Task<SalesInvoice?> GetByIdWithItemsAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SalesInvoices
            .AsNoTracking()
            .Include(invoice => invoice.Customer)
            .Include(invoice => invoice.Items)
                .ThenInclude(item => item.Part)
            .FirstOrDefaultAsync(invoice => invoice.Id == invoiceId, cancellationToken);
    }
}

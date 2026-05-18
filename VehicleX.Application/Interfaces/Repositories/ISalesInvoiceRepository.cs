using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface ISalesInvoiceRepository
{
    Task AddAsync(SalesInvoice salesInvoice, CancellationToken cancellationToken = default);

    Task<SalesInvoice?> GetByIdWithItemsAsync(int invoiceId, CancellationToken cancellationToken = default);

    Task<IEnumerable<SalesInvoice>> GetByDateRangeAsync(DateTime from, DateTime to);

    Task<IEnumerable<SalesInvoice>> GetAllWithCustomerAsync();

    Task<List<SalesInvoice>> GetByCustomerIdWithItemsAsync(
        int customerId,
        CancellationToken cancellationToken = default);
}

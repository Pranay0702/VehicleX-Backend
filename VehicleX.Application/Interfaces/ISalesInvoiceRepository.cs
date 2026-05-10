using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface ISalesInvoiceRepository
{
    Task AddAsync(SalesInvoice salesInvoice, CancellationToken cancellationToken = default);

    Task<SalesInvoice?> GetByIdWithItemsAsync(int invoiceId, CancellationToken cancellationToken = default);
}

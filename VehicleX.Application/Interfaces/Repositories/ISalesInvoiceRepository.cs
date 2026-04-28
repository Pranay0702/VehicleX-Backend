using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface ISalesInvoiceRepository
{
    // Get all sales invoices within a date range
    Task<IEnumerable<SalesInvoice>> GetByDateRangeAsync(DateTime from, DateTime to);

    // Get all invoices including customer info
    Task<IEnumerable<SalesInvoice>> GetAllWithCustomerAsync();
}
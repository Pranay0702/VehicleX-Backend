using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface IPurchaseInvoiceRepository
{
    // Get all purchase invoices within a date range 
    Task<IEnumerable<PurchaseInvoice>> GetByDateRangeAsync(DateTime from, DateTime to);
}
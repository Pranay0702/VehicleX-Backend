using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface IPurchaseRepository
{
    Task<IEnumerable<PurchaseInvoice>> GetAllAsync();
    
    Task<PurchaseInvoice?> GetByIdAsync(int id);
    
    Task<PurchaseInvoice> CreatePurchaseAsync(PurchaseInvoice invoice, List<Part> partsToUpdate);

    Task<int> GetTodaysPurchaseCountAsync();
}
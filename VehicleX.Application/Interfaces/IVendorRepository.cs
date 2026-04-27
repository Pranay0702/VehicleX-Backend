using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface IVendorRepository
{
    Task<IEnumerable<Vendor>> GetAllAsync();

    Task<Vendor?> GetByIdAsync(int id);

    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);

    Task<Vendor> AddAsync(Vendor vendor);

    Task UpdateAsync(Vendor vendor);

    Task DeleteAsync(Vendor vendor);
}
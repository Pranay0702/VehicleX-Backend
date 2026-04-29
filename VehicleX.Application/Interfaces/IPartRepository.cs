using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface IPartRepository
{
    Task<IEnumerable<Part>> GetAllAsync();

    Task<Part?> GetByIdAsync(int id);

    Task<bool> ExistsByPartNumberAsync(string partNumber, int? excludeId = null);

    Task<bool> AnyByVendorIdAsync(int vendorId);

    Task<Part> AddAsync(Part part);

    Task UpdateAsync(Part part);

    Task DeleteAsync(Part part);
}
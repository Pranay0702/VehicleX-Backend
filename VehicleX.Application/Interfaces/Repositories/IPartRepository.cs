using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface IPartRepository
{
    Task<IEnumerable<Part>> GetAllAsync();

    Task<Part?> GetByIdAsync(int id);

    Task<List<Part>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    Task<bool> ExistsByPartNumberAsync(string partNumber, int? excludeId = null);

    Task<bool> AnyByVendorIdAsync(int vendorId);

    Task<Part> AddAsync(Part part);

    Task UpdateAsync(Part part);

    Task DeleteAsync(Part part);
}

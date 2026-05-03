using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface IStaffRepository
{
    Task<IReadOnlyList<Staff>> GetAllAsync(CancellationToken cancellationToken);
    Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(string email, int? excludedStaffId, CancellationToken cancellationToken);
    Task AddAsync(Staff staff, CancellationToken cancellationToken);
    void Update(Staff staff);
    void Delete(Staff staff);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

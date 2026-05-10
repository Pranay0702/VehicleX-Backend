using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface IServiceReviewRepository
{
    Task<IReadOnlyList<ServiceReview>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceReview>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<ServiceReview?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> AppointmentReviewExistsAsync(int appointmentId, CancellationToken cancellationToken);
    Task AddAsync(ServiceReview serviceReview, CancellationToken cancellationToken);
    void Update(ServiceReview serviceReview);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

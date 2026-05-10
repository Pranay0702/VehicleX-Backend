using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Appointment>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
    void Update(Appointment appointment);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

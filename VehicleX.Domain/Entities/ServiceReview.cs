using VehicleX.Domain.Enums;

namespace VehicleX.Domain.Entities;

public class ServiceReview
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int? AppointmentId { get; set; }
    public ServiceRating Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Customer? Customer { get; set; }
    public Appointment? Appointment { get; set; }
}

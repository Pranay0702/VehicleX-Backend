using VehicleX.Domain.Enums;

namespace VehicleX.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime AppointmentDateUtc { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }
    public string? VehicleRegistrationNumber { get; set; }
    public string? Notes { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Customer? Customer { get; set; }
    public ServiceReview? ServiceReview { get; set; }
}

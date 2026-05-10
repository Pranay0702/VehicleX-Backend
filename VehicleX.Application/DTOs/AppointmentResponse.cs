using VehicleX.Domain.Enums;

namespace VehicleX.Application.DTOs;

public class AppointmentResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime AppointmentDateUtc { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }
    public string? VehicleRegistrationNumber { get; set; }
    public string? Notes { get; set; }
    public AppointmentStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

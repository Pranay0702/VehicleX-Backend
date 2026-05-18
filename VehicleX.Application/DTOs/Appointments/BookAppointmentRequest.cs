using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Appointments;

public class BookAppointmentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than zero.")]
    public int CustomerId { get; set; }

    [Required]
    public DateTime AppointmentDateUtc { get; set; }

    [Required, StringLength(120, MinimumLength = 2)]
    public string ServiceType { get; set; } = string.Empty;

    [StringLength(80)]
    public string? VehicleMake { get; set; }

    [StringLength(80)]
    public string? VehicleModel { get; set; }

    [StringLength(30)]
    public string? VehicleRegistrationNumber { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

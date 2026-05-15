using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Customers;

public class UpdateCustomerVehicleDto
{
    [Required]
    [MaxLength(30)]
    public string VehicleNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int? Year { get; set; }

    [MaxLength(30)]
    public string? FuelType { get; set; }
}
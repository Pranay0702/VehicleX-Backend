using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Customers;

public class CreateVehicleDto
{
    [Required(ErrorMessage = "Vehicle number is required.")]
    [StringLength(30, ErrorMessage = "Vehicle number cannot exceed 30 characters.")]
    public string VehicleNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Brand is required.")]
    [StringLength(50, ErrorMessage = "Brand cannot exceed 50 characters.")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model is required.")]
    [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters.")]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Year must be valid.")]
    public int? Year { get; set; }

    [StringLength(30, ErrorMessage = "Fuel type cannot exceed 30 characters.")]
    public string? FuelType { get; set; }
}
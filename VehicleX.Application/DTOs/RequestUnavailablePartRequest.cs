using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs;

public class RequestUnavailablePartRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than zero.")]
    public int CustomerId { get; set; }

    [Required, StringLength(160, MinimumLength = 2)]
    public string PartName { get; set; } = string.Empty;

    [StringLength(80)]
    public string? PartNumber { get; set; }

    [StringLength(80)]
    public string? VehicleMake { get; set; }

    [StringLength(80)]
    public string? VehicleModel { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

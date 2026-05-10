using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Part;

public class CreatePartDto
{
    [Required(ErrorMessage = "Part name is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Part name must be between 2 and 200 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Part number is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Part number must be between 2 and 50 characters.")]
    public string PartNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Stock quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    public int StockQuantity { get; set; }
    
    [Required(ErrorMessage = "Vendor ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Vendor ID must be a valid positive number.")]
    public int VendorId { get; set; }
}
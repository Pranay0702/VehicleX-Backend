using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Purchase;

public class PurchaseItemDto
{

    [Required(ErrorMessage = "Part ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Part ID must be a valid positive number.")]
    public int PartId { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Unit price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
    public decimal UnitPrice { get; set; }
}
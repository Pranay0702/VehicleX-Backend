using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Purchase;

public class CreatePurchaseDto
{

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }
    
    [Required(ErrorMessage = "At least one purchase item is required.")]
    [MinLength(1, ErrorMessage = "At least one purchase item is required.")]
    public List<PurchaseItemDto> Items { get; set; } = new();
}
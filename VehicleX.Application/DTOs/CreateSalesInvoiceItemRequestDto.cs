using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs;

public class CreateSalesInvoiceItemRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "PartId must be greater than 0.")]
    public int PartId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
}

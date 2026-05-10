using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs;

public class CreateSalesInvoiceRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than 0.")]
    public int CustomerId { get; set; }

    [MinLength(1, ErrorMessage = "At least one sales item is required.")]
    public List<CreateSalesInvoiceItemRequestDto> Items { get; set; } = new();
}

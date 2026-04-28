namespace VehicleX.Application.DTOs;

public class SalesInvoiceResponseDto
{
    public int InvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal SubTotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public List<SalesInvoiceItemResponseDto> Items { get; set; } = new();
}

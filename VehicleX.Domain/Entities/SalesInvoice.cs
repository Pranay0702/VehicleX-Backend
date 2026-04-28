namespace VehicleX.Domain.Entities;

public class SalesInvoice
{
    public int Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public decimal SubTotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public Customer? Customer { get; set; }

    public ICollection<SalesItem> Items { get; set; } = new List<SalesItem>();
}

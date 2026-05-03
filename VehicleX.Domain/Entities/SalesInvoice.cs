namespace VehicleX.Domain.Entities;

public class SalesInvoice
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public bool IsCredit { get; set; } = false;

    public bool IsCreditPaid { get; set; } = false;

    public Customer Customer { get; set; } = null!;

    public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();
}
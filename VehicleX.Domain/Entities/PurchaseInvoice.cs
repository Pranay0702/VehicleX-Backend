namespace VehicleX.Domain.Entities;

public class PurchaseInvoice
{
    public int Id { get; set; }

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
}
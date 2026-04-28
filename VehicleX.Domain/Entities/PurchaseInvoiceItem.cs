namespace VehicleX.Domain.Entities;

public class PurchaseInvoiceItem
{
    public int Id { get; set; }

    public int PurchaseInvoiceId { get; set; }

    public int PartId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;
}
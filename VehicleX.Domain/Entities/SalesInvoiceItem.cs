namespace VehicleX.Domain.Entities;

public class SalesInvoiceItem
{
    public int Id { get; set; }

    public int SalesInvoiceId { get; set; }

    public int PartId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public SalesInvoice SalesInvoice { get; set; } = null!;
}
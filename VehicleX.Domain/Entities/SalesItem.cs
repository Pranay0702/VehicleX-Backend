namespace VehicleX.Domain.Entities;

public class SalesItem
{
    public int Id { get; set; }

    public int SalesInvoiceId { get; set; }

    public int PartId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    public SalesInvoice? SalesInvoice { get; set; }

    public Part? Part { get; set; }
}

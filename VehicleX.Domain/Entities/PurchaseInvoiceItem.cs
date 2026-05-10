using VehicleX.Domain.Common;

namespace VehicleX.Domain.Entities;

public class PurchaseInvoiceItem : BaseEntity
{
    
    
    public int PurchaseInvoiceId { get; set; }
    
    public int PartId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal TotalPrice { get; set; }

    // Navigation Properties
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;
    public Part Part { get; set; } = null!;
}
using VehicleX.Domain.Common;

namespace VehicleX.Domain.Entities;

public class Part : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int VendorId { get; set; }

    // Navigation property
    public Vendor Vendor { get; set; } = null!;
    
    public ICollection<PurchaseInvoiceItem> PurchaseItems { get; set; } = new List<PurchaseInvoiceItem>();
}
using VehicleX.Domain.Enums;

namespace VehicleX.Domain.Entities;

public class CustomerPurchase
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime PurchaseDateUtc { get; set; }
    public decimal TotalAmount { get; set; }
    public PurchaseStatus Status { get; set; } = PurchaseStatus.Pending;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<CustomerPurchaseItem> Items { get; set; } = new List<CustomerPurchaseItem>();
}

namespace VehicleX.Domain.Entities;

public class CustomerPurchaseItem
{
    public int Id { get; set; }
    public int CustomerPurchaseId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public CustomerPurchase? CustomerPurchase { get; set; }
}

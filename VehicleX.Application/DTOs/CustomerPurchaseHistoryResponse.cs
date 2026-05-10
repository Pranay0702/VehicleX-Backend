using VehicleX.Domain.Enums;

namespace VehicleX.Application.DTOs;

public class CustomerPurchaseHistoryResponse
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime PurchaseDateUtc { get; set; }
    public decimal TotalAmount { get; set; }
    public PurchaseStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public IReadOnlyList<CustomerPurchaseItemResponse> Items { get; set; } = [];
}

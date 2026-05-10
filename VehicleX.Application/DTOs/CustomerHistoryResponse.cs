namespace VehicleX.Application.DTOs;

public class CustomerHistoryResponse
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public IReadOnlyList<CustomerPurchaseHistoryResponse> PurchaseHistory { get; set; } = [];
    public IReadOnlyList<CustomerServiceHistoryResponse> ServiceHistory { get; set; } = [];
}

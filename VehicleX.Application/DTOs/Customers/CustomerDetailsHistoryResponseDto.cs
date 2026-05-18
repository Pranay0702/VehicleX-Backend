namespace VehicleX.Application.DTOs.Customers;

public class CustomerDetailsHistoryResponseDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime CustomerSinceUtc { get; set; }
    public List<VehicleResponseDto> Vehicles { get; set; } = new();
    public List<CustomerPurchaseHistoryDto> PurchaseHistory { get; set; } = new();
    public bool IsServiceHistoryAvailable { get; set; }
    public List<CustomerServiceHistoryDto> ServiceHistory { get; set; } = new();
}

public class CustomerPurchaseHistoryDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDateUtc { get; set; }
    public decimal SubTotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCredit { get; set; }
    public bool IsCreditPaid { get; set; }
    public List<CustomerPurchaseHistoryItemDto> Items { get; set; } = new();
}

public class CustomerPurchaseHistoryItemDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class CustomerServiceHistoryDto
{
    public int ServiceId { get; set; }
    public DateTime ServiceDateUtc { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}

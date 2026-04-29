namespace VehicleX.Application.DTOs.Reports;

// Customers who come back frequently (sorted by visit count)
public class RegularCustomerDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalPurchases { get; set; }
}

// Customers who have spent the most money (sorted by spend)
public class HighSpenderDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalAmountSpent { get; set; }
}

// Customers who bought on credit and haven't paid yet
public class PendingCreditDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int UnpaidInvoiceCount { get; set; }
    public decimal TotalCreditOwed { get; set; }
}
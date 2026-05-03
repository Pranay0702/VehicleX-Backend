namespace VehicleX.Application.DTOs.Reports;

public class FinancialReportDto
{
    public string Period { get; set; } = string.Empty;

    public decimal TotalSalesRevenue { get; set; }

    public decimal TotalPurchaseCost { get; set; }

    // Net = Sales - Purchases
    public decimal NetProfit { get; set; }

    public int TotalSalesInvoices { get; set; }

    public int TotalPurchaseInvoices { get; set; }
}
namespace VehicleX.Application.DTOs.Email;

public class InvoiceItemEmailDto
{
    public string  PartName   { get; set; } = string.Empty;
    public string  PartNumber { get; set; } = string.Empty;
    public int     Quantity   { get; set; }
    public decimal UnitPrice  { get; set; }
    public decimal LineTotal  { get; set; }
}

public class LowStockItemDto
{
    public string PartName     { get; set; } = string.Empty;
    public string PartNumber   { get; set; } = string.Empty;
    public int    CurrentStock { get; set; }
}

public class UnpaidInvoiceDto
{
    public int      InvoiceId   { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal  Amount      { get; set; }
}
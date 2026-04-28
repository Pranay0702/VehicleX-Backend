namespace VehicleX.Domain.Entities;

public class Part
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int StockQuantity { get; set; }

    public ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
}

using VehicleX.Domain.Common;

namespace VehicleX.Domain.Entities;

/// <summary>
/// Represents a vehicle part in the inventory.
/// Fully implemented in Commit 3.
/// </summary>
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
}

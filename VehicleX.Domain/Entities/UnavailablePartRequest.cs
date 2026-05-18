using VehicleX.Domain.Enums;

namespace VehicleX.Domain.Entities;

public class UnavailablePartRequest
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public PartRequestStatus Status { get; set; } = PartRequestStatus.Pending;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public Customer? Customer { get; set; }
}

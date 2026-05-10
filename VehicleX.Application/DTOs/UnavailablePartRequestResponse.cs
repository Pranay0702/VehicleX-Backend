using VehicleX.Domain.Enums;

namespace VehicleX.Application.DTOs;

public class UnavailablePartRequestResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public string? PartNumber { get; set; }
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public PartRequestStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

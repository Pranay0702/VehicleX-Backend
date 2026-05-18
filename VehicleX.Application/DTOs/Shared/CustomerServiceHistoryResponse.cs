using VehicleX.Domain.Enums;
using VehicleX.Application.DTOs.ServiceReviews;

namespace VehicleX.Application.DTOs.Shared;

public class CustomerServiceHistoryResponse
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDateUtc { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public AppointmentStatus AppointmentStatus { get; set; }
    public string AppointmentStatusName { get; set; } = string.Empty;
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }
    public string? VehicleRegistrationNumber { get; set; }
    public ServiceReviewResponse? Review { get; set; }
}

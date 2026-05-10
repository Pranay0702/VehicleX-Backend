using VehicleX.Domain.Enums;

namespace VehicleX.Application.DTOs;

public class ServiceReviewResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int? AppointmentId { get; set; }
    public ServiceRating Rating { get; set; }
    public string RatingName { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

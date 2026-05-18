using System.ComponentModel.DataAnnotations;
using VehicleX.Domain.Enums;

namespace VehicleX.Application.DTOs.ServiceReviews;

public class CreateServiceReviewRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be greater than zero.")]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "AppointmentId must be greater than zero.")]
    public int? AppointmentId { get; set; }

    [Required]
    public ServiceRating Rating { get; set; }

    [Required, StringLength(1200, MinimumLength = 3)]
    public string Comment { get; set; } = string.Empty;
}

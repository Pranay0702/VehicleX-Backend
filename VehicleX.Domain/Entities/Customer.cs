namespace VehicleX.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<UnavailablePartRequest> UnavailablePartRequests { get; set; } = new List<UnavailablePartRequest>();
    public ICollection<ServiceReview> ServiceReviews { get; set; } = new List<ServiceReview>();
}

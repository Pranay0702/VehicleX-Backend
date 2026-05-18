namespace VehicleX.Domain.Entities;

public class Customer
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? PasswordHash { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<UnavailablePartRequest> UnavailablePartRequests { get; set; } = new List<UnavailablePartRequest>();
    public ICollection<ServiceReview> ServiceReviews { get; set; } = new List<ServiceReview>();
    public ICollection<CustomerPurchase> Purchases { get; set; } = new List<CustomerPurchase>();
}
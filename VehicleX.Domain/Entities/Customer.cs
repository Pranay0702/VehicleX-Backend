namespace VehicleX.Domain.Entities;

public class Customer
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
}

namespace VehicleX.Application.DTOs.Customers;

public class CustomerSearchResultDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<VehicleResponseDto> Vehicles { get; set; } = new();
}
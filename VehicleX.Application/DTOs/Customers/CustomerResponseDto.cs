namespace VehicleX.Application.DTOs.Customers;

public class CustomerResponseDto
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<VehicleResponseDto> Vehicles { get; set; } = new();
}

public class VehicleResponseDto
{
    public int Id { get; set; }

    public string VehicleNumber { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int? Year { get; set; }

    public string? FuelType { get; set; }
}
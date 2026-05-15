using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Customers;

public class UpdateCustomerProfileDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Address { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Customers;

public class CustomerSelfRegisterDto
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Phone number is not valid.")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
    public string Password { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Vehicle details are required.")]
    public CreateVehicleDto Vehicle { get; set; } = new();
}
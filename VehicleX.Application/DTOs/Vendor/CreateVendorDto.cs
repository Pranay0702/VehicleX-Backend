using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs.Vendor;

public class CreateVendorDto
{
    [Required(ErrorMessage = "Vendor name is required.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Vendor name must be between 2 and 200 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact person is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Contact person name must be between 2 and 150 characters.")]
    public string ContactPerson { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "A valid phone number is required.")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    public string Address { get; set; } = string.Empty;
}
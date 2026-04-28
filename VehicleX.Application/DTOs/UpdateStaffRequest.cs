using System.ComponentModel.DataAnnotations;
using VehicleX.Domain.Enums;

namespace VehicleX.Application.DTOs;

public class UpdateStaffRequest
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Phone, StringLength(30)]
    public string? PhoneNumber { get; set; }

    [Required]
    public StaffRole Role { get; set; }

    public bool IsActive { get; set; } = true;
}

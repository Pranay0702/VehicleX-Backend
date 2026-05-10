using System.ComponentModel.DataAnnotations;

namespace VehicleX.Application.DTOs;

public class CreateCustomerRequest
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;
}

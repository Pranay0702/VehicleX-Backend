using System.ComponentModel.DataAnnotations;
using VehicleX.Domain.Enums;

namespace VehicleX.Application.DTOs;

public class UpdateStaffRoleRequest
{
    [Required]
    public StaffRole Role { get; set; }
}

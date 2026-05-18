using System.ComponentModel.DataAnnotations;
using VehicleX.Domain.Enums;

namespace VehicleX.Application.DTOs.Staff;

public class UpdateStaffRoleRequest
{
    [Required]
    public StaffRole Role { get; set; }
}

using VehicleX.Domain.Common;
namespace VehicleX.Domain.Entities;

public class Vendor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string ContactPerson { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public ICollection<Part> Parts { get; set; } = new List<Part>();
}
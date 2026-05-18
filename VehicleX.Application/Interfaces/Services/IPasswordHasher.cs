namespace VehicleX.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
}

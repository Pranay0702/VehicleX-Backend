namespace VehicleX.Application.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
}

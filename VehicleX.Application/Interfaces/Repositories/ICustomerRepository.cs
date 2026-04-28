using VehicleX.Domain.Entities;

namespace VehicleX.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer> AddAsync(Customer customer);

    Task<bool> PhoneNumberExistsAsync(string phoneNumber);

    Task<bool> EmailExistsAsync(string email);
}
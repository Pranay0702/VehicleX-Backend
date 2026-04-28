using Microsoft.EntityFrameworkCore;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> AddAsync(Customer customer) //Save customer and vehicle data into database.
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();

        return customer;
    }

    public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)  //Check if the phone number is already used.
    {
        return await _context.Customers
            .AnyAsync(c => c.PhoneNumber == phoneNumber);
    }

    public async Task<bool> EmailExistsAsync(string email)  //Check if the email is already used.
    {
        return await _context.Customers
            .AnyAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
    }
}
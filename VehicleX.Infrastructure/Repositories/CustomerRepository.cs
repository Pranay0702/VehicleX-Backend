using Microsoft.EntityFrameworkCore;
using SalesCustomerRepository = VehicleX.Application.Interfaces.ICustomerRepository;
using CustomerRegistrationRepository = VehicleX.Application.Interfaces.Repositories.ICustomerRepository;
using VehicleX.Domain.Entities;
using VehicleX.Infrastructure.Data;

namespace VehicleX.Infrastructure.Repositories;

public class CustomerRepository : SalesCustomerRepository, CustomerRegistrationRepository
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

    public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .OrderBy(customer => customer.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);
    }
}

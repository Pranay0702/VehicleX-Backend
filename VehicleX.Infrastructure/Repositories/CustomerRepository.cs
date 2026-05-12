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

    public async Task<List<Customer>> SearchCustomersAsync(string searchTerm) //searches by credentials and vehicle number.
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<Customer>();
        }

        var term = searchTerm.Trim();

        var query = _context.Customers
            .Include(c => c.Vehicles)
            .AsNoTracking()
            .AsQueryable();

        if (int.TryParse(term, out var customerId))
        {
            query = query.Where(c => c.Id == customerId
                || EF.Functions.ILike(c.FullName, $"%{term}%")
                || EF.Functions.ILike(c.PhoneNumber, $"%{term}%")
                || EF.Functions.ILike(c.Email, $"%{term}%")
                || c.Vehicles.Any(v => EF.Functions.ILike(v.VehicleNumber, $"%{term}%")));
        }
        else
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.FullName, $"%{term}%")
                || EF.Functions.ILike(c.PhoneNumber, $"%{term}%")
                || EF.Functions.ILike(c.Email, $"%{term}%")
                || c.Vehicles.Any(v => EF.Functions.ILike(v.VehicleNumber, $"%{term}%")));
        }

        return await query
            .OrderBy(c => c.FullName)
            .ToListAsync();
    }
}

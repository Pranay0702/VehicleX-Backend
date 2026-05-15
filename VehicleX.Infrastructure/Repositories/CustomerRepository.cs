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

    public async Task<bool> VehicleNumberExistsAsync(string vehicleNumber)  //Check if the vehicle number is already used.
    {
        if (string.IsNullOrWhiteSpace(vehicleNumber))
        {
            return false;
        }

        var normalizedVehicleNumber = vehicleNumber.Trim().ToLower();

        return await _context.Vehicles
            .AnyAsync(v => v.VehicleNumber.ToLower() == normalizedVehicleNumber);
    }

    public async Task<Customer?> GetByIdWithVehiclesAsync(int customerId)
    {
        return await _context.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();

        return customer;
    }

    public async Task<List<Vehicle>> GetVehiclesByCustomerIdAsync(int customerId)
    {
        return await _context.Vehicles
            .AsNoTracking()
            .Where(v => v.CustomerId == customerId)
            .OrderBy(v => v.VehicleNumber)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetVehicleByIdAndCustomerIdAsync(int vehicleId, int customerId)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.CustomerId == customerId);
    }

    public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();

        return vehicle;
    }

    public async Task<Vehicle> UpdateVehicleAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();

        return vehicle;
    }

    public async Task<bool> DeleteVehicleAsync(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        var affectedRows = await _context.SaveChangesAsync();

        return affectedRows > 0;
    }

    public async Task<bool> VehicleNumberExistsForOtherCustomerAsync(string vehicleNumber, int customerId, int? vehicleId = null)
    {
        if (string.IsNullOrWhiteSpace(vehicleNumber))
        {
            return false;
        }

        var normalizedVehicleNumber = vehicleNumber.Trim().ToLower();

        return await _context.Vehicles.AnyAsync(v =>
            v.VehicleNumber.ToLower() == normalizedVehicleNumber
            && v.CustomerId != customerId
            && (!vehicleId.HasValue || v.Id != vehicleId.Value));
    }
}

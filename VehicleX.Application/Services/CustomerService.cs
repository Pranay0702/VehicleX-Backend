using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Customers;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ApiResponse<CustomerResponseDto>> StaffRegisterCustomerAsync(StaffRegisterCustomerDto dto)
    {
        if (await _customerRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
        {
            return ApiResponse<CustomerResponseDto>.Fail("Phone number already exists.");
        }

        if (await _customerRepository.EmailExistsAsync(dto.Email))
        {
            return ApiResponse<CustomerResponseDto>.Fail("Email already exists.");
        }

        var customer = new Customer
        {
            FullName = dto.FullName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Email = dto.Email.Trim(),
            Address = dto.Address?.Trim(),
            Vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    VehicleNumber = dto.Vehicle.VehicleNumber.Trim(),
                    Brand = dto.Vehicle.Brand.Trim(),
                    Model = dto.Vehicle.Model.Trim(),
                    Year = dto.Vehicle.Year,
                    FuelType = dto.Vehicle.FuelType?.Trim()
                }
            }
        };

        var savedCustomer = await _customerRepository.AddAsync(customer);

        var response = new CustomerResponseDto
        {
            Id = savedCustomer.Id,
            FullName = savedCustomer.FullName,
            PhoneNumber = savedCustomer.PhoneNumber,
            Email = savedCustomer.Email,
            Address = savedCustomer.Address,
            CreatedAt = savedCustomer.CreatedAt,
            Vehicles = savedCustomer.Vehicles.Select(v => new VehicleResponseDto
            {
                Id = v.Id,
                VehicleNumber = v.VehicleNumber,
                Brand = v.Brand,
                Model = v.Model,
                Year = v.Year,
                FuelType = v.FuelType
            }).ToList()
        };

        return ApiResponse<CustomerResponseDto>.Ok(response, "Customer registered successfully.");
    }
}
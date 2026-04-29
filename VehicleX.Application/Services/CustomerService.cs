using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Customers;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public CustomerService(
        ICustomerRepository customerRepository,
        IJwtTokenService jwtTokenService)
    {
        _customerRepository = customerRepository;
        _jwtTokenService = jwtTokenService;
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

    public async Task<ApiResponse<CustomerAuthResponseDto>> CustomerSelfRegisterAsync(CustomerSelfRegisterDto dto)
    {
        // Checking phone number so same customer is not registered twice
        if (await _customerRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
        {
            return ApiResponse<CustomerAuthResponseDto>.Fail("Phone number already exists.");
        }

        // Checking email because login/register should have unique email
        if (await _customerRepository.EmailExistsAsync(dto.Email))
        {
            return ApiResponse<CustomerAuthResponseDto>.Fail("Email already exists.");
        }

        var customer = new Customer
        {
            FullName = dto.FullName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Email = dto.Email.Trim(),
            Address = dto.Address?.Trim(),

            // Password is hashed before saving, real password is never stored
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),

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

        // Token is created after successful customer registration
        var token = _jwtTokenService.GenerateCustomerToken(savedCustomer);

        var response = new CustomerAuthResponseDto
        {
            Id = savedCustomer.Id,
            FullName = savedCustomer.FullName,
            PhoneNumber = savedCustomer.PhoneNumber,
            Email = savedCustomer.Email,
            Role = "Customer",
            Token = token
        };

        return ApiResponse<CustomerAuthResponseDto>.Ok(response, "Customer registered successfully.");
    }

    public async Task<ApiResponse<CustomerAuthResponseDto>> CustomerLoginAsync(CustomerLoginDto dto)
    {
        var customer = await _customerRepository.GetByEmailAsync(dto.Email);

        if (customer == null)
        {
            return new ApiResponse<CustomerAuthResponseDto>
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        if (string.IsNullOrWhiteSpace(customer.PasswordHash))
        {
            return new ApiResponse<CustomerAuthResponseDto>
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash);

        if (!isPasswordValid)
        {
            return new ApiResponse<CustomerAuthResponseDto>
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        var token = _jwtTokenService.GenerateCustomerToken(customer);

        var response = new CustomerAuthResponseDto
        {
            Id = customer.Id,
            FullName = customer.FullName,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            Role = "Customer",
            Token = token
        };

        return new ApiResponse<CustomerAuthResponseDto>
        {
            Success = true,
            Message = "Customer logged in successfully.",
            Data = response
        };
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleX.Application.DTOs.Customers;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // allows staff to register a new customer along with their vehicle information
    [HttpPost("staff-register")]
    public async Task<IActionResult> StaffRegisterCustomer([FromBody] StaffRegisterCustomerDto dto)
    {
        var result = await _customerService.StaffRegisterCustomerAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(StaffRegisterCustomer), new { id = result.Data?.Id }, result);
    }

    // allows customers to register themselves and receive a JWT token
    [HttpPost("self-register")]
    public async Task<IActionResult> CustomerSelfRegister([FromBody] CustomerSelfRegisterDto dto)
    {
        // Customer registers themselves and receives JWT token
        var result = await _customerService.CustomerSelfRegisterAsync(dto);

        if (!result.Success) 
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(CustomerSelfRegister), new { id = result.Data?.Id }, result);
    }

    // allows customers to log in using their email and password
    [HttpPost("login")]
    public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginDto dto)
    {
        var result = await _customerService.CustomerLoginAsync(dto);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    // searching for customers based on a search term, which can match customer credentials or vehicle numbers
    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers([FromQuery] string searchTerm)
    {
        var result = await _customerService.SearchCustomersAsync(searchTerm);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    // allows customers to retrieve their profile information, including their personal details and associated vehicles
    [Authorize(Roles = "Customer")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
        {
            return Unauthorized("Invalid customer token.");
        }

        var result = await _customerService.GetProfileAsync(customerId.Value);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    //  allows customers to update their profile information, such as their name, email, phone number, and other relevant details
    [Authorize(Roles = "Customer")]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileDto dto)
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
        {
            return Unauthorized("Invalid customer token.");
        }

        var result = await _customerService.UpdateProfileAsync(customerId.Value, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    //  allows customers to retrieve a list of their associated vehicles, including details such as make, model, year, and registration number
    [Authorize(Roles = "Customer")]
    [HttpGet("vehicles")]
    public async Task<IActionResult> GetMyVehicles()
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
        {
            return Unauthorized("Invalid customer token.");
        }

        var result = await _customerService.GetMyVehiclesAsync(customerId.Value);

        return Ok(result);
    }

    // allows customers to add a new vehicle to their profile by providing the necessary details, such as the vehicle's make, model, year, and registration number
    [Authorize(Roles = "Customer")]
    [HttpPost("vehicles")]
    public async Task<IActionResult> AddVehicle([FromBody] CreateCustomerVehicleDto dto)
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
        {
            return Unauthorized("Invalid customer token.");
        }

        var result = await _customerService.AddVehicleAsync(customerId.Value, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    // allows customers to update the details of an existing vehicle associated with their profile, such as changing the registration number or updating the vehicle's make and model information
    [Authorize(Roles = "Customer")]
    [HttpPut("vehicles/{vehicleId:int}")]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] UpdateCustomerVehicleDto dto)
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
        {
            return Unauthorized("Invalid customer token.");
        }

        var result = await _customerService.UpdateVehicleAsync(customerId.Value, vehicleId, dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    // allows customers to remove a vehicle from their profile, which may involve deleting the vehicle's information from the system or disassociating it from the customer's account
    [Authorize(Roles = "Customer")]
    [HttpDelete("vehicles/{vehicleId:int}")]
    public async Task<IActionResult> DeleteVehicle(int vehicleId)
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
        {
            return Unauthorized("Invalid customer token.");
        }

        var result = await _customerService.DeleteVehicleAsync(customerId.Value, vehicleId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    //  Helper method to extract the logged-in customer's ID from the JWT token claims
    private int? GetLoggedInCustomerId()
    {
        var customerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(customerIdClaim))
        {
            return null;
        }

        return int.TryParse(customerIdClaim, out var customerId)
            ? customerId
            : null;
    }
}
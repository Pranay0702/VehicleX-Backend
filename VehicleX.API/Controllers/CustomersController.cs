using Microsoft.AspNetCore.Mvc;
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
}
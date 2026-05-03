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

    [HttpPost("login")]
    public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginDto dto)
    {
        var result = await _customerService.CustomerLoginAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
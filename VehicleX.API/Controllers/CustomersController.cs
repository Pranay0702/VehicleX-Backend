using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleX.Application.Common;
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

    // -------------------------------------------------------------------------
    // Registration & Auth
    // -------------------------------------------------------------------------

    /// <summary>Allows staff to register a new customer along with their vehicle.</summary>
    [HttpPost("staff-register")]
    public async Task<IActionResult> StaffRegisterCustomer([FromBody] StaffRegisterCustomerDto dto)
    {
        var result = await _customerService.StaffRegisterCustomerAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { customerId = result.Data?.Id }, result);
    }

    /// <summary>Allows customers to register themselves and receive a JWT token.</summary>
    [HttpPost("self-register")]
    public async Task<IActionResult> CustomerSelfRegister([FromBody] CustomerSelfRegisterDto dto)
    {
        var result = await _customerService.CustomerSelfRegisterAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { customerId = result.Data?.Id }, result);
    }

    /// <summary>Allows customers to log in using their email and password.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> CustomerLogin([FromBody] CustomerLoginDto dto)
    {
        var result = await _customerService.CustomerLoginAsync(dto);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    // -------------------------------------------------------------------------
    // Staff-facing queries
    // -------------------------------------------------------------------------

    /// <summary>Returns all customers ordered by name.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CustomerResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _customerService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns a single customer by ID.</summary>
    [HttpGet("{customerId:int}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int customerId, CancellationToken cancellationToken)
    {
        var result = await _customerService.GetByIdAsync(customerId, cancellationToken);

        if (!result.Success)
        {
            return result.Message == "Customer not found."
                ? NotFound(result)
                : BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>Searches customers by name, email, phone, ID, or vehicle number.</summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers([FromQuery] string searchTerm)
    {
        var result = await _customerService.SearchCustomersAsync(searchTerm);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Returns full customer details, vehicles, and purchase history for staff.</summary>
    [HttpGet("staff/{customerId:int}/details-history")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDetailsHistoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerDetailsHistoryResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CustomerDetailsHistoryResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<CustomerDetailsHistoryResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomerDetailsAndHistoryForStaff(int customerId, CancellationToken cancellationToken)
    {
        if (customerId <= 0)
            return BadRequest(ApiResponse<CustomerDetailsHistoryResponseDto>.Fail("CustomerId must be greater than 0."));

        var result = await _customerService.GetCustomerDetailsAndHistoryForStaffAsync(customerId, cancellationToken);

        if (result.Success)
            return Ok(result);

        if (string.Equals(result.Message, "Customer not found.", StringComparison.OrdinalIgnoreCase))
            return NotFound(result);

        if (string.Equals(result.Message, "Unable to fetch customer details and history right now.", StringComparison.OrdinalIgnoreCase))
            return StatusCode(StatusCodes.Status500InternalServerError, result);

        return BadRequest(result);
    }

    // -------------------------------------------------------------------------
    // Customer self-service (profile & vehicles)
    // -------------------------------------------------------------------------

    /// <summary>Returns the logged-in customer's profile and associated vehicles.</summary>
    [Authorize(Roles = "Customer")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
            return Unauthorized("Invalid customer token.");

        var result = await _customerService.GetProfileAsync(customerId.Value);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>Updates the logged-in customer's profile information.</summary>
    [Authorize(Roles = "Customer")]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileDto dto)
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
            return Unauthorized("Invalid customer token.");

        var result = await _customerService.UpdateProfileAsync(customerId.Value, dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Returns all vehicles associated with the logged-in customer.</summary>
    [Authorize(Roles = "Customer")]
    [HttpGet("vehicles")]
    public async Task<IActionResult> GetMyVehicles()
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
            return Unauthorized("Invalid customer token.");

        var result = await _customerService.GetMyVehiclesAsync(customerId.Value);
        return Ok(result);
    }

    /// <summary>Adds a new vehicle to the logged-in customer's profile.</summary>
    [Authorize(Roles = "Customer")]
    [HttpPost("vehicles")]
    public async Task<IActionResult> AddVehicle([FromBody] CreateCustomerVehicleDto dto)
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
            return Unauthorized("Invalid customer token.");

        var result = await _customerService.AddVehicleAsync(customerId.Value, dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Updates an existing vehicle on the logged-in customer's profile.</summary>
    [Authorize(Roles = "Customer")]
    [HttpPut("vehicles/{vehicleId:int}")]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] UpdateCustomerVehicleDto dto)
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
            return Unauthorized("Invalid customer token.");

        var result = await _customerService.UpdateVehicleAsync(customerId.Value, vehicleId, dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Removes a vehicle from the logged-in customer's profile.</summary>
    [Authorize(Roles = "Customer")]
    [HttpDelete("vehicles/{vehicleId:int}")]
    public async Task<IActionResult> DeleteVehicle(int vehicleId)
    {
        var customerId = GetLoggedInCustomerId();

        if (customerId == null)
            return Unauthorized("Invalid customer token.");

        var result = await _customerService.DeleteVehicleAsync(customerId.Value, vehicleId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private int? GetLoggedInCustomerId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(claim))
            return null;

        return int.TryParse(claim, out var id) ? id : null;
    }
}
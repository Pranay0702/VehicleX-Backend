using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.DTOs.Common;
using VehicleX.Application.DTOs.Vendor;
using VehicleX.Application.Interfaces;

namespace VehicleX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly ILogger<VendorsController> _logger;

    public VendorsController(IVendorService vendorService, ILogger<VendorsController> logger)
    {
        _vendorService = vendorService ?? throw new ArgumentNullException(nameof(vendorService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// GET: api/vendors

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET /api/vendors - Retrieving all vendors.");
        var result = await _vendorService.GetAllVendorsAsync();
        return Ok(result);
    }
    
    /// GET: api/vendors/{id}

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET /api/vendors/{Id} - Retrieving vendor.", id);
        var result = await _vendorService.GetVendorByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
    
    /// POST: api/vendors

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendorDto dto)
    {
        _logger.LogInformation("POST /api/vendors - Creating new vendor.");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", errors));
        }

        var result = await _vendorService.CreateVendorAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }
    
    /// Updates an existing vendor.
    /// PUT: api/vendors/{id}
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVendorDto dto)
    {
        _logger.LogInformation("PUT /api/vendors/{Id} - Updating vendor.", id);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", errors));
        }

        var result = await _vendorService.UpdateVendorAsync(id, dto);

        if (!result.Success)
        {
            if (result.Message.Contains("not found"))
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
    
    /// Deletes a vendor by ID.
    /// DELETE: api/vendors/{id}

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("DELETE /api/vendors/{Id} - Deleting vendor.", id);
        var result = await _vendorService.DeleteVendorAsync(id);

        if (!result.Success)
        {
            if (result.Message.Contains("not found"))
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}

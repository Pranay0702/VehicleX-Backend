using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.DTOs.Common;
using VehicleX.Application.DTOs.Part;
using VehicleX.Application.Interfaces;

namespace VehicleX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;
    private readonly ILogger<PartsController> _logger;

    public PartsController(IPartService partService, ILogger<PartsController> logger)
    {
        _partService = partService ?? throw new ArgumentNullException(nameof(partService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// Retrieves all parts with vendor information.
    /// GET: api/parts
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET /api/parts - Retrieving all parts.");
        var result = await _partService.GetAllPartsAsync();
        return Ok(result);
    }

    /// Retrieves a part by ID.
    /// GET: api/parts/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET /api/parts/{Id} - Retrieving part.", id);
        var result = await _partService.GetPartByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// Creates a new part linked to a vendor.
    /// POST: api/parts
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartDto dto)
    {
        _logger.LogInformation("POST /api/parts - Creating new part.");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", errors));
        }

        var result = await _partService.CreatePartAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// Updates an existing part.
    /// PUT: api/parts/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartDto dto)
    {
        _logger.LogInformation("PUT /api/parts/{Id} - Updating part.", id);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", errors));
        }

        var result = await _partService.UpdatePartAsync(id, dto);

        if (!result.Success)
        {
            if (result.Message.Contains("not found"))
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// Deletes a part by ID.
    /// DELETE: api/parts/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("DELETE /api/parts/{Id} - Deleting part.", id);
        var result = await _partService.DeletePartAsync(id);

        if (!result.Success)
        {
            if (result.Message.Contains("not found"))
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}

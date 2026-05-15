using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Purchase;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;
    private readonly ILogger<PurchasesController> _logger;

    public PurchasesController(IPurchaseService purchaseService, ILogger<PurchasesController> logger)
    {
        _purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// Retrieves all purchase invoices
    /// GET: api/purchases
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET /api/purchases - Retrieving all purchase invoices.");
        var result = await _purchaseService.GetAllPurchasesAsync();
        return Ok(result);
    }

    /// Retrieves a purchase invoice by ID with all items
    /// GET: api/purchases/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GET /api/purchases/{Id} - Retrieving purchase invoice.", id);
        var result = await _purchaseService.GetPurchaseByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
    
    /// Creates a new purchase invoice with items then Validates parts, calculates totals, and updates stock
    /// POST: api/purchases
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
    {
        _logger.LogInformation("POST /api/purchases - Creating new purchase invoice.");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(ApiResponse<object>.FailureResponse("Validation failed.", errors));
        }

        var result = await _purchaseService.CreatePurchaseAsync(dto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }
}

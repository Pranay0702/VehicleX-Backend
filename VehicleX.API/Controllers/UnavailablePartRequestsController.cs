using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;

namespace VehicleX.Controllers;

[ApiController]
[Route("api/unavailable-part-requests")]
public class UnavailablePartRequestsController : ApiControllerBase
{
    private readonly IUnavailablePartRequestService _partRequestService;

    public UnavailablePartRequestsController(IUnavailablePartRequestService partRequestService)
    {
        _partRequestService = partRequestService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UnavailablePartRequestResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _partRequestService.GetAllAsync(cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UnavailablePartRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UnavailablePartRequestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UnavailablePartRequestResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _partRequestService.GetByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("customer/{customerId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UnavailablePartRequestResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UnavailablePartRequestResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UnavailablePartRequestResponse>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCustomerId(int customerId, CancellationToken cancellationToken)
    {
        var result = await _partRequestService.GetByCustomerIdAsync(customerId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UnavailablePartRequestResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UnavailablePartRequestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UnavailablePartRequestResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] RequestUnavailablePartRequest request, CancellationToken cancellationToken)
    {
        var result = await _partRequestService.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, ApiResponse<UnavailablePartRequestResponse>.Ok(result.Data, result.Message));
    }
}

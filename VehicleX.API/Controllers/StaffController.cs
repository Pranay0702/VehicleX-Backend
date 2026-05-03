using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;

namespace VehicleX.Controllers;

[ApiController]
[Route("api/staff")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StaffResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _staffService.GetAllAsync(cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _staffService.GetByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateStaffRequest request, CancellationToken cancellationToken)
    {
        var result = await _staffService.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, ApiResponse<StaffResponse>.Ok(result.Data, result.Message));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStaffRequest request, CancellationToken cancellationToken)
    {
        var result = await _staffService.UpdateAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPatch("{id:int}/role")]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<StaffResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateStaffRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _staffService.UpdateRoleAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _staffService.DeleteAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    private ObjectResult ToActionResult<T>(ServiceResult<T> result)
    {
        var response = result.Success
            ? ApiResponse<T>.Ok(result.Data, result.Message)
            : ApiResponse<T>.Fail(result.Message, result.Errors);

        return StatusCode(result.StatusCode, response);
    }
}

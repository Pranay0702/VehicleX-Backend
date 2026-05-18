using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;

namespace VehicleX.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult<T>(ApiResponse<T> result)
    {
        if (result.Success)
            return Ok(result);

        if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(result);

        if (result.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)
            || result.Message.Contains("already been reviewed", StringComparison.OrdinalIgnoreCase))
            return Conflict(result);

        if (result.Message.Contains("Unable to", StringComparison.OrdinalIgnoreCase))
            return StatusCode(StatusCodes.Status500InternalServerError, result);

        return BadRequest(result);
    }
}
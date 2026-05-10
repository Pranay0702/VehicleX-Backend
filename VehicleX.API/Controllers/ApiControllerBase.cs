using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;

namespace VehicleX.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected ObjectResult ToActionResult<T>(ServiceResult<T> result)
    {
        var response = result.Success
            ? ApiResponse<T>.Ok(result.Data, result.Message)
            : ApiResponse<T>.Fail(result.Message, result.Errors);

        return StatusCode(result.StatusCode, response);
    }
}

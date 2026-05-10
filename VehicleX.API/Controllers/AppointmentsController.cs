using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;

namespace VehicleX.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ApiControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AppointmentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetAllAsync(cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("customer/{customerId:int}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AppointmentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AppointmentResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AppointmentResponse>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCustomerId(int customerId, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetByCustomerIdAsync(customerId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AppointmentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Book([FromBody] BookAppointmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.BookAsync(request, cancellationToken);
        if (!result.Success)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, ApiResponse<AppointmentResponse>.Ok(result.Data, result.Message));
    }
}

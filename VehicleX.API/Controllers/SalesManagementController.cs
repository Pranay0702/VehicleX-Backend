using Microsoft.AspNetCore.Mvc;
using VehicleX.API.Common;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;

namespace VehicleX.API.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesManagementController : ControllerBase
{
    private readonly ISalesManagementService _salesManagementService;

    public SalesManagementController(ISalesManagementService salesManagementService)
    {
        _salesManagementService = salesManagementService;
    }

    [HttpGet("customers")]
    [ProducesResponseType(typeof(ApiResponse<List<CustomerLookupDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken)
    {
        var result = await _salesManagementService.GetCustomersAsync(cancellationToken);
        return MapResult(result);
    }

    [HttpGet("parts")]
    [ProducesResponseType(typeof(ApiResponse<List<PartLookupDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetParts(CancellationToken cancellationToken)
    {
        var result = await _salesManagementService.GetPartsAsync(cancellationToken);
        return MapResult(result);
    }

    [HttpPost("invoices")]
    [ProducesResponseType(typeof(ApiResponse<SalesInvoiceResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSalesInvoice(
        [FromBody] CreateSalesInvoiceRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _salesManagementService.CreateSalesInvoiceAsync(request, cancellationToken);

        if (result.IsSuccess && result.Data is not null)
        {
            var successResponse = ApiResponse<SalesInvoiceResponseDto>.SuccessResponse(result.Message, result.Data);

            return CreatedAtAction(
                nameof(GetSalesInvoiceById),
                new { invoiceId = result.Data.InvoiceId },
                successResponse);
        }

        return MapResult(result);
    }

    [HttpGet("invoices/{invoiceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<SalesInvoiceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSalesInvoiceById(int invoiceId, CancellationToken cancellationToken)
    {
        var result = await _salesManagementService.GetSalesInvoiceByIdAsync(invoiceId, cancellationToken);
        return MapResult(result);
    }

    private IActionResult MapResult<T>(ServiceResult<T> result)
    {
        if (result.IsSuccess && result.Data is not null)
        {
            return Ok(ApiResponse<T>.SuccessResponse(result.Message, result.Data));
        }

        var errorResponse = ApiResponse<object>.Failure(result.Message, new[] { result.Message });

        return result.ErrorType switch
        {
            ResultErrorType.Validation => BadRequest(errorResponse),
            ResultErrorType.NotFound => NotFound(errorResponse),
            ResultErrorType.Conflict => Conflict(errorResponse),
            _ => StatusCode(StatusCodes.Status500InternalServerError, errorResponse)
        };
    }
}

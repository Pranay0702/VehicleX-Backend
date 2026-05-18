using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Shared;
using VehicleX.Application.DTOs.Sales;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesManagementController : ApiControllerBase
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
        return ToActionResult(result);
    }

    [HttpGet("parts")]
    [ProducesResponseType(typeof(ApiResponse<List<PartLookupDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetParts(CancellationToken cancellationToken)
    {
        var result = await _salesManagementService.GetPartsAsync(cancellationToken);
        return ToActionResult(result);
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

        if (!result.Success)
            return ToActionResult(result);

        return CreatedAtAction(
            nameof(GetSalesInvoiceById),
            new { invoiceId = result.Data!.InvoiceId },
            ApiResponse<SalesInvoiceResponseDto>.Ok(result.Data, result.Message));
    }

    [HttpGet("invoices/{invoiceId:int}")]
    [ProducesResponseType(typeof(ApiResponse<SalesInvoiceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSalesInvoiceById(int invoiceId, CancellationToken cancellationToken)
    {
        var result = await _salesManagementService.GetSalesInvoiceByIdAsync(invoiceId, cancellationToken);
        return ToActionResult(result);
    }
}
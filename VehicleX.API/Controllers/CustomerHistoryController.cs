using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Shared;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.Controllers;

[ApiController]
[Route("api/customers/{customerId:int}/history")]
public class CustomerHistoryController : ApiControllerBase
{
    private readonly ICustomerHistoryService _customerHistoryService;

    public CustomerHistoryController(ICustomerHistoryService customerHistoryService)
    {
        _customerHistoryService = customerHistoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CustomerHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CustomerHistoryResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CustomerHistoryResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(int customerId, CancellationToken cancellationToken)
    {
        var result = await _customerHistoryService.GetHistoryAsync(customerId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("purchases")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CustomerPurchaseHistoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CustomerPurchaseHistoryResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CustomerPurchaseHistoryResponse>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchaseHistory(int customerId, CancellationToken cancellationToken)
    {
        var result = await _customerHistoryService.GetPurchaseHistoryAsync(customerId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("services")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CustomerServiceHistoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CustomerServiceHistoryResponse>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CustomerServiceHistoryResponse>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServiceHistory(int customerId, CancellationToken cancellationToken)
    {
        var result = await _customerHistoryService.GetServiceHistoryAsync(customerId, cancellationToken);
        return ToActionResult(result);
    }
}

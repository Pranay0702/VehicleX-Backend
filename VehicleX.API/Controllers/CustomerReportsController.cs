using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.API.Controllers;

[Route("api/reports/customers")]
[ApiController]
public class CustomerReportsController : ControllerBase
{
    private readonly ICustomerReportService _reportService;
    private readonly ILogger<CustomerReportsController> _logger;

    public CustomerReportsController(
        ICustomerReportService reportService,
        ILogger<CustomerReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    // GET api/reports/customers/regulars?minPurchases=3
    [HttpGet("regulars")]
    public async Task<IActionResult> GetRegularCustomers([FromQuery] int minPurchases = 3)
    {
        if (minPurchases < 1)
            return BadRequest(new { success = false, message = "minPurchases must be at least 1." });

        _logger.LogInformation("Fetching regular customers with min {Min} purchases", minPurchases);
        var result = await _reportService.GetRegularCustomersAsync(minPurchases);
        return Ok(result);
    }

    // GET api/reports/customers/high-spenders?minAmount=5000
    [HttpGet("high-spenders")]
    public async Task<IActionResult> GetHighSpenders([FromQuery] decimal minAmount = 5000)
    {
        if (minAmount < 0)
            return BadRequest(new { success = false, message = "minAmount cannot be negative." });

        _logger.LogInformation("Fetching high spenders with min spend {Amount}", minAmount);
        var result = await _reportService.GetHighSpendersAsync(minAmount);
        return Ok(result);
    }

    // GET api/reports/customers/pending-credits
    [HttpGet("pending-credits")]
    public async Task<IActionResult> GetPendingCredits()
    {
        _logger.LogInformation("Fetching customers with pending credit invoices");
        var result = await _reportService.GetPendingCreditsAsync();
        return Ok(result);
    }
}
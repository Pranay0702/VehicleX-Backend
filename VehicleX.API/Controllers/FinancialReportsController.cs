using Microsoft.AspNetCore.Mvc;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.API.Controllers;

[Route("api/reports/financial")]
[ApiController]
public class FinancialReportsController : ControllerBase
{
    private readonly IFinancialReportService _reportService;
    private readonly ILogger<FinancialReportsController> _logger;

    public FinancialReportsController(
        IFinancialReportService reportService,
        ILogger<FinancialReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    // GET api/reports/financial/daily?date=2026-04-27
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyReport([FromQuery] DateTime date)
    {
        if (date == default)
            return BadRequest(new { success = false, message = "A valid date is required. Format: yyyy-MM-dd" });

        _logger.LogInformation("Generating daily financial report for {Date}", date.ToString("yyyy-MM-dd"));
        var result = await _reportService.GetDailyReportAsync(date);
        return Ok(result);
    }

    // GET api/reports/financial/monthly?year=2026&month=4
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyReport([FromQuery] int year, [FromQuery] int month)
    {
        if (year < 2000 || year > 2100)
            return BadRequest(new { success = false, message = "Year must be between 2000 and 2100." });

        if (month < 1 || month > 12)
            return BadRequest(new { success = false, message = "Month must be between 1 and 12." });

        _logger.LogInformation("Generating monthly financial report for {Year}-{Month}", year, month);
        var result = await _reportService.GetMonthlyReportAsync(year, month);
        return Ok(result);
    }

    // GET api/reports/financial/yearly?year=2026
    [HttpGet("yearly")]
    public async Task<IActionResult> GetYearlyReport([FromQuery] int year)
    {
        if (year < 2000 || year > 2100)
            return BadRequest(new { success = false, message = "Year must be between 2000 and 2100." });

        _logger.LogInformation("Generating yearly financial report for {Year}", year);
        var result = await _reportService.GetYearlyReportAsync(year);
        return Ok(result);
    }
}
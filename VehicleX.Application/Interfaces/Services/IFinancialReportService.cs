using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Reports;

namespace VehicleX.Application.Interfaces.Services;

public interface IFinancialReportService
{
    Task<ApiResponse<FinancialReportDto>> GetDailyReportAsync(DateTime date);

    Task<ApiResponse<FinancialReportDto>> GetMonthlyReportAsync(int year, int month);

    Task<ApiResponse<FinancialReportDto>> GetYearlyReportAsync(int year);
}
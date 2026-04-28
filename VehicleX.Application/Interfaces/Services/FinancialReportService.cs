using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Reports;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.Application.Services;

public class FinancialReportService : IFinancialReportService
{
    private readonly ISalesInvoiceRepository _salesRepo;
    private readonly IPurchaseInvoiceRepository _purchaseRepo;

    public FinancialReportService(
        ISalesInvoiceRepository salesRepo,
        IPurchaseInvoiceRepository purchaseRepo)
    {
        _salesRepo = salesRepo;
        _purchaseRepo = purchaseRepo;
    }

    public async Task<ApiResponse<FinancialReportDto>> GetDailyReportAsync(DateTime date)
    {
        // From midnight to midnight of that day
        var from = date.Date;
        var to = from.AddDays(1);
        var report = await BuildReportAsync(from, to, $"Daily: {from:yyyy-MM-dd}");
        return ApiResponse<FinancialReportDto>.Ok(report);
    }

    public async Task<ApiResponse<FinancialReportDto>> GetMonthlyReportAsync(int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);
        var report = await BuildReportAsync(from, to, $"Monthly: {from:yyyy-MM}");
        return ApiResponse<FinancialReportDto>.Ok(report);
    }

    public async Task<ApiResponse<FinancialReportDto>> GetYearlyReportAsync(int year)
    {
        var from = new DateTime(year, 1, 1);
        var to = from.AddYears(1);
        var report = await BuildReportAsync(from, to, $"Yearly: {year}");
        return ApiResponse<FinancialReportDto>.Ok(report);
    }

    // avoids repeating logic across daily/monthly/yearly
    private async Task<FinancialReportDto> BuildReportAsync(DateTime from, DateTime to, string period)
    {
        var sales = (await _salesRepo.GetByDateRangeAsync(from, to)).ToList();
        var purchases = (await _purchaseRepo.GetByDateRangeAsync(from, to)).ToList();

        var totalSales = sales.Sum(s => s.TotalAmount);
        var totalPurchases = purchases.Sum(p => p.TotalAmount);

        return new FinancialReportDto
        {
            Period = period,
            TotalSalesRevenue = totalSales,
            TotalPurchaseCost = totalPurchases,
            NetProfit = totalSales - totalPurchases,
            TotalSalesInvoices = sales.Count,
            TotalPurchaseInvoices = purchases.Count
        };
    }
}
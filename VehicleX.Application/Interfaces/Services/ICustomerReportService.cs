using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Reports;

namespace VehicleX.Application.Interfaces.Services;

public interface ICustomerReportService
{
    // Customers with at least minPurchases number of invoices
    Task<ApiResponse<IEnumerable<RegularCustomerDto>>> GetRegularCustomersAsync(int minPurchases);

    // Customers who have spent at least minAmount in total
    Task<ApiResponse<IEnumerable<HighSpenderDto>>> GetHighSpendersAsync(decimal minAmount);

    // Customers with unpaid credit invoices
    Task<ApiResponse<IEnumerable<PendingCreditDto>>> GetPendingCreditsAsync();
}
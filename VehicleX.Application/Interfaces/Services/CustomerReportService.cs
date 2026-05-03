using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Reports;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;

namespace VehicleX.Application.Services;

public class CustomerReportService : ICustomerReportService
{
    private readonly ISalesInvoiceRepository _salesRepo;

    public CustomerReportService(ISalesInvoiceRepository salesRepo)
    {
        _salesRepo = salesRepo;
    }

    public async Task<ApiResponse<IEnumerable<RegularCustomerDto>>> GetRegularCustomersAsync(int minPurchases)
    {
        // Load all invoices with their customer data
        var invoices = (await _salesRepo.GetAllWithCustomerAsync()).ToList();

        var result = invoices
            .GroupBy(i => i.Customer)
            .Where(g => g.Count() >= minPurchases)
            .Select(g => new RegularCustomerDto
            {
                CustomerId   = g.Key.Id,
                FullName     = g.Key.FullName,
                PhoneNumber  = g.Key.PhoneNumber,
                Email        = g.Key.Email,
                TotalPurchases = g.Count()
            })
            .OrderByDescending(c => c.TotalPurchases);

        return ApiResponse<IEnumerable<RegularCustomerDto>>.Ok(result,
            $"Found {result.Count()} regular customer(s) with {minPurchases}+ purchases.");
    }

    public async Task<ApiResponse<IEnumerable<HighSpenderDto>>> GetHighSpendersAsync(decimal minAmount)
    {
        var invoices = (await _salesRepo.GetAllWithCustomerAsync()).ToList();

        var result = invoices
            .GroupBy(i => i.Customer)
            .Select(g => new HighSpenderDto
            {
                CustomerId       = g.Key.Id,
                FullName         = g.Key.FullName,
                PhoneNumber      = g.Key.PhoneNumber,
                Email            = g.Key.Email,
                TotalAmountSpent = g.Sum(i => i.TotalAmount)
            })
            .Where(c => c.TotalAmountSpent >= minAmount)
            .OrderByDescending(c => c.TotalAmountSpent);

        return ApiResponse<IEnumerable<HighSpenderDto>>.Ok(result,
            $"Found {result.Count()} high spender(s) with spend ≥ {minAmount:C}.");
    }

    public async Task<ApiResponse<IEnumerable<PendingCreditDto>>> GetPendingCreditsAsync()
    {
        var invoices = (await _salesRepo.GetAllWithCustomerAsync()).ToList();

        // Only credit invoices that have NOT been paid yet
        var result = invoices
            .Where(i => i.IsCredit && !i.IsCreditPaid)
            .GroupBy(i => i.Customer)
            .Select(g => new PendingCreditDto
            {
                CustomerId         = g.Key.Id,
                FullName           = g.Key.FullName,
                PhoneNumber        = g.Key.PhoneNumber,
                Email              = g.Key.Email,
                Address            = g.Key.Address,
                UnpaidInvoiceCount = g.Count(),
                TotalCreditOwed    = g.Sum(i => i.TotalAmount)
            })
            .OrderByDescending(c => c.TotalCreditOwed);

        return ApiResponse<IEnumerable<PendingCreditDto>>.Ok(result,
            $"Found {result.Count()} customer(s) with pending credit.");
    }
}
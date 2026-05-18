using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Sales;
using VehicleX.Application.DTOs.Shared;

namespace VehicleX.Application.Interfaces.Services;

public interface ISalesManagementService
{
    Task<ApiResponse<List<CustomerLookupDto>>> GetCustomersAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<List<PartLookupDto>>> GetPartsAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<SalesInvoiceResponseDto>> CreateSalesInvoiceAsync(
        CreateSalesInvoiceRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SalesInvoiceResponseDto>> GetSalesInvoiceByIdAsync(
        int invoiceId,
        CancellationToken cancellationToken = default);
}

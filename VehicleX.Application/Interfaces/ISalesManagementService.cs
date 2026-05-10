using VehicleX.Application.Common;
using VehicleX.Application.DTOs;

namespace VehicleX.Application.Interfaces;

public interface ISalesManagementService
{
    Task<ServiceResult<List<CustomerLookupDto>>> GetCustomersAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<List<PartLookupDto>>> GetPartsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<SalesInvoiceResponseDto>> CreateSalesInvoiceAsync(
        CreateSalesInvoiceRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<SalesInvoiceResponseDto>> GetSalesInvoiceByIdAsync(
        int invoiceId,
        CancellationToken cancellationToken = default);
}

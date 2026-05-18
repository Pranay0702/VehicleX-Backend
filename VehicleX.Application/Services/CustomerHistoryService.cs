using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Shared;
using VehicleX.Application.DTOs.ServiceReviews;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class CustomerHistoryService : ICustomerHistoryService
{
    private readonly ICustomerRepository        _customerRepository;
    private readonly ICustomerPurchaseRepository _purchaseRepository;
    private readonly IAppointmentRepository     _appointmentRepository;

    public CustomerHistoryService(
        ICustomerRepository customerRepository,
        ICustomerPurchaseRepository purchaseRepository,
        IAppointmentRepository appointmentRepository)
    {
        _customerRepository    = customerRepository;
        _purchaseRepository    = purchaseRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<ApiResponse<CustomerHistoryResponse>> GetHistoryAsync(int customerId, CancellationToken cancellationToken)
    {
        if (customerId <= 0)
            return ApiResponse<CustomerHistoryResponse>.Fail("Invalid customer id.");

        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
            return ApiResponse<CustomerHistoryResponse>.Fail("Customer was not found.");

        var purchases    = await _purchaseRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId, cancellationToken);

        return ApiResponse<CustomerHistoryResponse>.Ok(new CustomerHistoryResponse
        {
            CustomerId      = customer.Id,
            CustomerName    = customer.FullName,
            PurchaseHistory = purchases.Select(ToPurchaseHistoryResponse).ToList(),
            ServiceHistory  = appointments.Select(ToServiceHistoryResponse).ToList()
        }, "Customer history retrieved successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<CustomerPurchaseHistoryResponse>>> GetPurchaseHistoryAsync(int customerId, CancellationToken cancellationToken)
    {
        if (customerId <= 0)
            return ApiResponse<IReadOnlyList<CustomerPurchaseHistoryResponse>>.Fail("Invalid customer id.");

        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
            return ApiResponse<IReadOnlyList<CustomerPurchaseHistoryResponse>>.Fail("Customer was not found.");

        var purchases = await _purchaseRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return ApiResponse<IReadOnlyList<CustomerPurchaseHistoryResponse>>.Ok(
            purchases.Select(ToPurchaseHistoryResponse).ToList(),
            "Customer purchase history retrieved successfully.");
    }

    public async Task<ApiResponse<IReadOnlyList<CustomerServiceHistoryResponse>>> GetServiceHistoryAsync(int customerId, CancellationToken cancellationToken)
    {
        if (customerId <= 0)
            return ApiResponse<IReadOnlyList<CustomerServiceHistoryResponse>>.Fail("Invalid customer id.");

        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
            return ApiResponse<IReadOnlyList<CustomerServiceHistoryResponse>>.Fail("Customer was not found.");

        var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return ApiResponse<IReadOnlyList<CustomerServiceHistoryResponse>>.Ok(
            appointments.Select(ToServiceHistoryResponse).ToList(),
            "Customer service history retrieved successfully.");
    }

    private static CustomerPurchaseHistoryResponse ToPurchaseHistoryResponse(CustomerPurchase purchase) => new()
    {
        Id              = purchase.Id,
        InvoiceNumber   = purchase.InvoiceNumber,
        PurchaseDateUtc = purchase.PurchaseDateUtc,
        TotalAmount     = purchase.TotalAmount,
        Status          = purchase.Status,
        StatusName      = purchase.Status.ToString(),
        Items           = purchase.Items.OrderBy(i => i.Id).Select(ToPurchaseItemResponse).ToList()
    };

    private static CustomerPurchaseItemResponse ToPurchaseItemResponse(CustomerPurchaseItem item) => new()
    {
        Id         = item.Id,
        PartName   = item.PartName,
        PartNumber = item.PartNumber,
        Quantity   = item.Quantity,
        UnitPrice  = item.UnitPrice,
        LineTotal  = item.LineTotal
    };

    private static CustomerServiceHistoryResponse ToServiceHistoryResponse(Appointment a) => new()
    {
        AppointmentId             = a.Id,
        AppointmentDateUtc        = a.AppointmentDateUtc,
        ServiceType               = a.ServiceType,
        AppointmentStatus         = a.Status,
        AppointmentStatusName     = a.Status.ToString(),
        VehicleMake               = a.VehicleMake,
        VehicleModel              = a.VehicleModel,
        VehicleRegistrationNumber = a.VehicleRegistrationNumber,
        Review                    = a.ServiceReview is null ? null : ToReviewResponse(a.ServiceReview)
    };

    private static ServiceReviewResponse ToReviewResponse(ServiceReview review) => new()
    {
        Id            = review.Id,
        CustomerId    = review.CustomerId,
        CustomerName  = review.Customer?.FullName ?? string.Empty,
        AppointmentId = review.AppointmentId,
        Rating        = review.Rating,
        RatingName    = review.Rating.ToString(),
        Comment       = review.Comment,
        CreatedAtUtc  = review.CreatedAtUtc,
        UpdatedAtUtc  = review.UpdatedAtUtc
    };
}

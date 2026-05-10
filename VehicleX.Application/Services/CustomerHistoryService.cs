using System.Net;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class CustomerHistoryService : ICustomerHistoryService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerPurchaseRepository _purchaseRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public CustomerHistoryService(
        ICustomerRepository customerRepository,
        ICustomerPurchaseRepository purchaseRepository,
        IAppointmentRepository appointmentRepository)
    {
        _customerRepository = customerRepository;
        _purchaseRepository = purchaseRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<ServiceResult<CustomerHistoryResponse>> GetHistoryAsync(int customerId, CancellationToken cancellationToken)
    {
        try
        {
            var customerResult = await GetCustomerAsync(customerId, cancellationToken);
            if (!customerResult.Success || customerResult.Data is null)
            {
                return ServiceResult<CustomerHistoryResponse>.Fail(customerResult.Message, customerResult.StatusCode, customerResult.Errors);
            }

            var purchases = await _purchaseRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            var customer = customerResult.Data;

            var response = new CustomerHistoryResponse
            {
                CustomerId = customer.Id,
                CustomerName = $"{customer.FirstName} {customer.LastName}".Trim(),
                PurchaseHistory = purchases.Select(ToPurchaseHistoryResponse).ToList(),
                ServiceHistory = appointments.Select(ToServiceHistoryResponse).ToList()
            };

            return ServiceResult<CustomerHistoryResponse>.Ok(response, "Customer history retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CustomerPurchaseHistoryResponse>>> GetPurchaseHistoryAsync(int customerId, CancellationToken cancellationToken)
    {
        try
        {
            var customerResult = await GetCustomerAsync(customerId, cancellationToken);
            if (!customerResult.Success)
            {
                return ServiceResult<IReadOnlyList<CustomerPurchaseHistoryResponse>>.Fail(customerResult.Message, customerResult.StatusCode, customerResult.Errors);
            }

            var purchases = await _purchaseRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            return ServiceResult<IReadOnlyList<CustomerPurchaseHistoryResponse>>.Ok(
                purchases.Select(ToPurchaseHistoryResponse).ToList(),
                "Customer purchase history retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<IReadOnlyList<CustomerServiceHistoryResponse>>> GetServiceHistoryAsync(int customerId, CancellationToken cancellationToken)
    {
        try
        {
            var customerResult = await GetCustomerAsync(customerId, cancellationToken);
            if (!customerResult.Success)
            {
                return ServiceResult<IReadOnlyList<CustomerServiceHistoryResponse>>.Fail(customerResult.Message, customerResult.StatusCode, customerResult.Errors);
            }

            var appointments = await _appointmentRepository.GetByCustomerIdAsync(customerId, cancellationToken);
            return ServiceResult<IReadOnlyList<CustomerServiceHistoryResponse>>.Ok(
                appointments.Select(ToServiceHistoryResponse).ToList(),
                "Customer service history retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<ServiceResult<Customer>> GetCustomerAsync(int customerId, CancellationToken cancellationToken)
    {
        if (customerId <= 0)
        {
            return ServiceResult<Customer>.Fail("Invalid customer id.", (int)HttpStatusCode.BadRequest);
        }

        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
        {
            return ServiceResult<Customer>.Fail("Customer was not found.", (int)HttpStatusCode.NotFound);
        }

        return ServiceResult<Customer>.Ok(customer);
    }

    private static CustomerPurchaseHistoryResponse ToPurchaseHistoryResponse(CustomerPurchase purchase)
    {
        return new CustomerPurchaseHistoryResponse
        {
            Id = purchase.Id,
            InvoiceNumber = purchase.InvoiceNumber,
            PurchaseDateUtc = purchase.PurchaseDateUtc,
            TotalAmount = purchase.TotalAmount,
            Status = purchase.Status,
            StatusName = purchase.Status.ToString(),
            Items = purchase.Items
                .OrderBy(item => item.Id)
                .Select(ToPurchaseItemResponse)
                .ToList()
        };
    }

    private static CustomerPurchaseItemResponse ToPurchaseItemResponse(CustomerPurchaseItem item)
    {
        return new CustomerPurchaseItemResponse
        {
            Id = item.Id,
            PartName = item.PartName,
            PartNumber = item.PartNumber,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal
        };
    }

    private static CustomerServiceHistoryResponse ToServiceHistoryResponse(Appointment appointment)
    {
        return new CustomerServiceHistoryResponse
        {
            AppointmentId = appointment.Id,
            AppointmentDateUtc = appointment.AppointmentDateUtc,
            ServiceType = appointment.ServiceType,
            AppointmentStatus = appointment.Status,
            AppointmentStatusName = appointment.Status.ToString(),
            VehicleMake = appointment.VehicleMake,
            VehicleModel = appointment.VehicleModel,
            VehicleRegistrationNumber = appointment.VehicleRegistrationNumber,
            Review = appointment.ServiceReview is null ? null : ToReviewResponse(appointment.ServiceReview)
        };
    }

    private static ServiceReviewResponse ToReviewResponse(ServiceReview review)
    {
        return new ServiceReviewResponse
        {
            Id = review.Id,
            CustomerId = review.CustomerId,
            CustomerName = review.Customer is null ? string.Empty : $"{review.Customer.FirstName} {review.Customer.LastName}".Trim(),
            AppointmentId = review.AppointmentId,
            Rating = review.Rating,
            RatingName = review.Rating.ToString(),
            Comment = review.Comment,
            CreatedAtUtc = review.CreatedAtUtc,
            UpdatedAtUtc = review.UpdatedAtUtc
        };
    }
}

using Microsoft.Extensions.Logging;
using VehicleX.Application.DTOs.Common;
using VehicleX.Application.DTOs.Vendor;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IPartRepository _partRepository;
    private readonly ILogger<VendorService> _logger;

    public VendorService(
        IVendorRepository vendorRepository,
        IPartRepository partRepository,
        ILogger<VendorService> logger)
    {
        _vendorRepository = vendorRepository ?? throw new ArgumentNullException(nameof(vendorRepository));
        _partRepository = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<ApiResponse<IEnumerable<VendorResponseDto>>> GetAllVendorsAsync()
    {
        _logger.LogInformation("Retrieving all vendors.");

        var vendors = await _vendorRepository.GetAllAsync();
        var vendorDtos = vendors.Select(MapToResponseDto);

        _logger.LogInformation("Successfully retrieved {Count} vendors.", vendorDtos.Count());
        return ApiResponse<IEnumerable<VendorResponseDto>>.SuccessResponse(vendorDtos, "Vendors retrieved successfully.");
    }
    
    public async Task<ApiResponse<VendorResponseDto>> GetVendorByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving vendor with ID {VendorId}.", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid vendor ID: {VendorId}.", id);
            return ApiResponse<VendorResponseDto>.FailureResponse("Invalid vendor ID. ID must be a positive number.");
        }

        var vendor = await _vendorRepository.GetByIdAsync(id);

        if (vendor == null)
        {
            _logger.LogWarning("Vendor with ID {VendorId} not found.", id);
            return ApiResponse<VendorResponseDto>.FailureResponse($"Vendor with ID {id} not found.");
        }

        _logger.LogInformation("Successfully retrieved vendor with ID {VendorId}.", id);
        return ApiResponse<VendorResponseDto>.SuccessResponse(MapToResponseDto(vendor), "Vendor retrieved successfully.");
    }
    
    public async Task<ApiResponse<VendorResponseDto>> CreateVendorAsync(CreateVendorDto dto)
    {
        _logger.LogInformation("Creating new vendor with name '{VendorName}'.", dto.Name);

        // Check for duplicate email
        if (await _vendorRepository.ExistsByEmailAsync(dto.Email))
        {
            _logger.LogWarning("Vendor with email '{Email}' already exists.", dto.Email);
            return ApiResponse<VendorResponseDto>.FailureResponse($"A vendor with email '{dto.Email}' already exists.");
        }

        var vendor = new Vendor
        {
            Name = dto.Name.Trim(),
            ContactPerson = dto.ContactPerson.Trim(),
            Email = dto.Email.Trim().ToLower(),
            Phone = dto.Phone.Trim(),
            Address = dto.Address.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdVendor = await _vendorRepository.AddAsync(vendor);

        _logger.LogInformation("Successfully created vendor with ID {VendorId}.", createdVendor.Id);
        return ApiResponse<VendorResponseDto>.SuccessResponse(MapToResponseDto(createdVendor), "Vendor created successfully.");
    }
    
    public async Task<ApiResponse<VendorResponseDto>> UpdateVendorAsync(int id, UpdateVendorDto dto)
    {
        _logger.LogInformation("Updating vendor with ID {VendorId}.", id);

        if (id <= 0)
        {
            return ApiResponse<VendorResponseDto>.FailureResponse("Invalid vendor ID. ID must be a positive number.");
        }

        var vendor = await _vendorRepository.GetByIdAsync(id);

        if (vendor == null)
        {
            _logger.LogWarning("Vendor with ID {VendorId} not found for update.", id);
            return ApiResponse<VendorResponseDto>.FailureResponse($"Vendor with ID {id} not found.");
        }

        // Check for duplicate email (exclude current vendor)
        if (await _vendorRepository.ExistsByEmailAsync(dto.Email, id))
        {
            _logger.LogWarning("Another vendor with email '{Email}' already exists.", dto.Email);
            return ApiResponse<VendorResponseDto>.FailureResponse($"Another vendor with email '{dto.Email}' already exists.");
        }
        
        vendor.Name = dto.Name.Trim();
        vendor.ContactPerson = dto.ContactPerson.Trim();
        vendor.Email = dto.Email.Trim().ToLower();
        vendor.Phone = dto.Phone.Trim();
        vendor.Address = dto.Address.Trim();
        vendor.UpdatedAt = DateTime.UtcNow;

        await _vendorRepository.UpdateAsync(vendor);

        _logger.LogInformation("Successfully updated vendor with ID {VendorId}.", id);
        return ApiResponse<VendorResponseDto>.SuccessResponse(MapToResponseDto(vendor), "Vendor updated successfully.");
    }
    
    public async Task<ApiResponse<bool>> DeleteVendorAsync(int id)
    {
        _logger.LogInformation("Deleting vendor with ID {VendorId}.", id);

        if (id <= 0)
        {
            return ApiResponse<bool>.FailureResponse("Invalid vendor ID. ID must be a positive number.");
        }

        var vendor = await _vendorRepository.GetByIdAsync(id);

        if (vendor == null)
        {
            _logger.LogWarning("Vendor with ID {VendorId} not found for deletion.", id);
            return ApiResponse<bool>.FailureResponse($"Vendor with ID {id} not found.");
        }

        // Check if vendor has associated parts
        if (await _partRepository.AnyByVendorIdAsync(id))
        {
            _logger.LogWarning("Cannot delete vendor {VendorId} because it has associated parts.", id);
            return ApiResponse<bool>.FailureResponse(
                "Cannot delete this vendor because it has associated parts. Please delete or reassign the parts first.");
        }

        await _vendorRepository.DeleteAsync(vendor);

        _logger.LogInformation("Successfully deleted vendor with ID {VendorId}.", id);
        return ApiResponse<bool>.SuccessResponse(true, "Vendor deleted successfully.");
    }
    
    private static VendorResponseDto MapToResponseDto(Vendor vendor)
    {
        return new VendorResponseDto
        {
            Id = vendor.Id,
            Name = vendor.Name,
            ContactPerson = vendor.ContactPerson,
            Email = vendor.Email,
            Phone = vendor.Phone,
            Address = vendor.Address,
            CreatedAt = vendor.CreatedAt,
            UpdatedAt = vendor.UpdatedAt,
            PartsCount = vendor.Parts?.Count ?? 0
        };
    }
}

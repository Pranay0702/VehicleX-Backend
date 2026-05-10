using Microsoft.Extensions.Logging;
using VehicleX.Application.DTOs.Common;
using VehicleX.Application.DTOs.Part;
using VehicleX.Application.Interfaces;
using VehicleX.Domain.Entities;

namespace VehicleX.Application.Services;

public class PartService : IPartService
{
    private readonly IPartRepository _partRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly ILogger<PartService> _logger;

    public PartService(
        IPartRepository partRepository,
        IVendorRepository vendorRepository,
        ILogger<PartService> logger)
    {
        _partRepository = partRepository ?? throw new ArgumentNullException(nameof(partRepository));
        _vendorRepository = vendorRepository ?? throw new ArgumentNullException(nameof(vendorRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ApiResponse<IEnumerable<PartResponseDto>>> GetAllPartsAsync()
    {
        _logger.LogInformation("Retrieving all parts.");

        var parts = await _partRepository.GetAllAsync();
        var partDtos = parts.Select(MapToResponseDto);

        _logger.LogInformation("Successfully retrieved {Count} parts.", partDtos.Count());
        return ApiResponse<IEnumerable<PartResponseDto>>.SuccessResponse(partDtos, "Parts retrieved successfully.");
    }
    
    public async Task<ApiResponse<PartResponseDto>> GetPartByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving part with ID {PartId}.", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid part ID: {PartId}.", id);
            return ApiResponse<PartResponseDto>.FailureResponse("Invalid part ID. ID must be a positive number.");
        }

        var part = await _partRepository.GetByIdAsync(id);

        if (part == null)
        {
            _logger.LogWarning("Part with ID {PartId} not found.", id);
            return ApiResponse<PartResponseDto>.FailureResponse($"Part with ID {id} not found.");
        }

        _logger.LogInformation("Successfully retrieved part with ID {PartId}.", id);
        return ApiResponse<PartResponseDto>.SuccessResponse(MapToResponseDto(part), "Part retrieved successfully.");
    }
    
    public async Task<ApiResponse<PartResponseDto>> CreatePartAsync(CreatePartDto dto)
    {
        _logger.LogInformation("Creating new part '{PartName}' with part number '{PartNumber}'.", dto.Name, dto.PartNumber);

        // Validate vendor exists
        var vendor = await _vendorRepository.GetByIdAsync(dto.VendorId);
        if (vendor == null)
        {
            _logger.LogWarning("Vendor with ID {VendorId} not found while creating part.", dto.VendorId);
            return ApiResponse<PartResponseDto>.FailureResponse($"Vendor with ID {dto.VendorId} does not exist.");
        }

        // Check for duplicate part number
        if (await _partRepository.ExistsByPartNumberAsync(dto.PartNumber))
        {
            _logger.LogWarning("Part with number '{PartNumber}' already exists.", dto.PartNumber);
            return ApiResponse<PartResponseDto>.FailureResponse($"A part with number '{dto.PartNumber}' already exists.");
        }

        var part = new Part
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            PartNumber = dto.PartNumber.Trim().ToUpper(),
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            VendorId = dto.VendorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdPart = await _partRepository.AddAsync(part);

        // Reload with vendor navigation property
        var reloadedPart = await _partRepository.GetByIdAsync(createdPart.Id);

        _logger.LogInformation("Successfully created part with ID {PartId}.", createdPart.Id);
        return ApiResponse<PartResponseDto>.SuccessResponse(
            MapToResponseDto(reloadedPart!), "Part created successfully.");
    }

    public async Task<ApiResponse<PartResponseDto>> UpdatePartAsync(int id, UpdatePartDto dto)
    {
        _logger.LogInformation("Updating part with ID {PartId}.", id);

        if (id <= 0)
        {
            return ApiResponse<PartResponseDto>.FailureResponse("Invalid part ID. ID must be a positive number.");
        }

        var part = await _partRepository.GetByIdAsync(id);

        if (part == null)
        {
            _logger.LogWarning("Part with ID {PartId} not found for update.", id);
            return ApiResponse<PartResponseDto>.FailureResponse($"Part with ID {id} not found.");
        }

        // Validate vendor exists
        var vendor = await _vendorRepository.GetByIdAsync(dto.VendorId);
        if (vendor == null)
        {
            _logger.LogWarning("Vendor with ID {VendorId} not found while updating part.", dto.VendorId);
            return ApiResponse<PartResponseDto>.FailureResponse($"Vendor with ID {dto.VendorId} does not exist.");
        }

        // Check for duplicate part number (exclude current part)
        if (await _partRepository.ExistsByPartNumberAsync(dto.PartNumber, id))
        {
            _logger.LogWarning("Another part with number '{PartNumber}' already exists.", dto.PartNumber);
            return ApiResponse<PartResponseDto>.FailureResponse($"Another part with number '{dto.PartNumber}' already exists.");
        }

        // Update properties
        part.Name = dto.Name.Trim();
        part.Description = dto.Description.Trim();
        part.PartNumber = dto.PartNumber.Trim().ToUpper();
        part.Price = dto.Price;
        part.StockQuantity = dto.StockQuantity;
        part.VendorId = dto.VendorId;
        part.UpdatedAt = DateTime.UtcNow;

        await _partRepository.UpdateAsync(part);

        // Reload with vendor info
        var updatedPart = await _partRepository.GetByIdAsync(id);

        _logger.LogInformation("Successfully updated part with ID {PartId}.", id);
        return ApiResponse<PartResponseDto>.SuccessResponse(
            MapToResponseDto(updatedPart!), "Part updated successfully.");
    }

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> DeletePartAsync(int id)
    {
        _logger.LogInformation("Deleting part with ID {PartId}.", id);

        if (id <= 0)
        {
            return ApiResponse<bool>.FailureResponse("Invalid part ID. ID must be a positive number.");
        }

        var part = await _partRepository.GetByIdAsync(id);

        if (part == null)
        {
            _logger.LogWarning("Part with ID {PartId} not found for deletion.", id);
            return ApiResponse<bool>.FailureResponse($"Part with ID {id} not found.");
        }

        await _partRepository.DeleteAsync(part);

        _logger.LogInformation("Successfully deleted part with ID {PartId}.", id);
        return ApiResponse<bool>.SuccessResponse(true, "Part deleted successfully.");
    }
    
    private static PartResponseDto MapToResponseDto(Part part)
    {
        return new PartResponseDto
        {
            Id = part.Id,
            Name = part.Name,
            Description = part.Description,
            PartNumber = part.PartNumber,
            Price = part.Price,
            StockQuantity = part.StockQuantity,
            VendorId = part.VendorId,
            VendorName = part.Vendor?.Name ?? "Unknown",
            CreatedAt = part.CreatedAt,
            UpdatedAt = part.UpdatedAt
        };
    }
}

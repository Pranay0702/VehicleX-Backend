using VehicleX.Application.Common;
using VehicleX.Application.DTOs.Staff;
using VehicleX.Application.Interfaces.Repositories;
using VehicleX.Application.Interfaces.Services;
using VehicleX.Domain.Entities;
using VehicleX.Domain.Enums;

namespace VehicleX.Application.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IPasswordHasher _passwordHasher;

    public StaffService(IStaffRepository staffRepository, IPasswordHasher passwordHasher)
    {
        _staffRepository = staffRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<IReadOnlyList<StaffResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _staffRepository.GetAllAsync(cancellationToken);
            return ApiResponse<IReadOnlyList<StaffResponse>>.Ok(staff.Select(ToResponse).ToList(), "Staff records retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ApiResponse<StaffResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ApiResponse<StaffResponse>.Fail("Invalid staff id.");
            }

            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff is null)
            {
                return ApiResponse<StaffResponse>.Fail("Staff member was not found.");
            }

            return ApiResponse<StaffResponse>.Ok(ToResponse(staff), "Staff record retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ApiResponse<StaffResponse>> CreateAsync(CreateStaffRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateRole(request.Role);
            if (validationErrors.Count > 0)
            {
                return ApiResponse<StaffResponse>.Fail("Validation failed.", validationErrors);
            }

            var email = NormalizeEmail(request.Email);
            if (await _staffRepository.EmailExistsAsync(email, null, cancellationToken))
            {
                return ApiResponse<StaffResponse>.Fail("A staff member with this email already exists.");
            }

            var staff = new Staff
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PhoneNumber = NormalizeOptional(request.PhoneNumber),
                Role = request.Role,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _staffRepository.AddAsync(staff, cancellationToken);
            await _staffRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<StaffResponse>.Ok(ToResponse(staff), "Staff member registered successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ApiResponse<StaffResponse>> UpdateAsync(int id, UpdateStaffRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ApiResponse<StaffResponse>.Fail("Invalid staff id.");
            }

            var validationErrors = ValidateRole(request.Role);
            if (validationErrors.Count > 0)
            {
                return ApiResponse<StaffResponse>.Fail("Validation failed.", validationErrors);
            }

            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff is null)
            {
                return ApiResponse<StaffResponse>.Fail("Staff member was not found.");
            }

            var email = NormalizeEmail(request.Email);
            if (await _staffRepository.EmailExistsAsync(email, id, cancellationToken))
            {
                return ApiResponse<StaffResponse>.Fail("A staff member with this email already exists.");
            }

            staff.FirstName = request.FirstName.Trim();
            staff.LastName = request.LastName.Trim();
            staff.Email = email;
            staff.PhoneNumber = NormalizeOptional(request.PhoneNumber);
            staff.Role = request.Role;
            staff.IsActive = request.IsActive;
            staff.UpdatedAtUtc = DateTime.UtcNow;

            _staffRepository.Update(staff);
            await _staffRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<StaffResponse>.Ok(ToResponse(staff), "Staff member updated successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ApiResponse<StaffResponse>> UpdateRoleAsync(int id, UpdateStaffRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ApiResponse<StaffResponse>.Fail("Invalid staff id.");
            }

            var validationErrors = ValidateRole(request.Role);
            if (validationErrors.Count > 0)
            {
                return ApiResponse<StaffResponse>.Fail("Validation failed.", validationErrors);
            }

            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff is null)
            {
                return ApiResponse<StaffResponse>.Fail("Staff member was not found.");
            }

            staff.Role = request.Role;
            staff.UpdatedAtUtc = DateTime.UtcNow;

            _staffRepository.Update(staff);
            await _staffRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<StaffResponse>.Ok(ToResponse(staff), "Staff role updated successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ApiResponse<object>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ApiResponse<object>.Fail("Invalid staff id.");
            }

            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff is null)
            {
                return ApiResponse<object>.Fail("Staff member was not found.");
            }

            _staffRepository.Delete(staff);
            await _staffRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<object>.Ok(null, "Staff member deleted successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static StaffResponse ToResponse(Staff staff)
    {
        return new StaffResponse
        {
            Id = staff.Id,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            FullName = $"{staff.FirstName} {staff.LastName}".Trim(),
            Email = staff.Email,
            PhoneNumber = staff.PhoneNumber,
            Role = staff.Role,
            RoleName = staff.Role.ToString(),
            IsActive = staff.IsActive,
            CreatedAtUtc = staff.CreatedAtUtc,
            UpdatedAtUtc = staff.UpdatedAtUtc
        };
    }

    private static Dictionary<string, string[]> ValidateRole(StaffRole role)
    {
        var errors = new Dictionary<string, string[]>();
        if (!Enum.IsDefined(typeof(StaffRole), role))
        {
            errors["role"] = ["Invalid staff role."];
        }

        return errors;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

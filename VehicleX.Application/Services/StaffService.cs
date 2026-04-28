using System.Net;
using VehicleX.Application.Common;
using VehicleX.Application.DTOs;
using VehicleX.Application.Interfaces;
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

    public async Task<ServiceResult<IReadOnlyList<StaffResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var staff = await _staffRepository.GetAllAsync(cancellationToken);
            return ServiceResult<IReadOnlyList<StaffResponse>>.Ok(staff.Select(ToResponse).ToList(), "Staff records retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<StaffResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<StaffResponse>.Fail("Invalid staff id.", (int)HttpStatusCode.BadRequest);
            }

            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff is null)
            {
                return ServiceResult<StaffResponse>.Fail("Staff member was not found.", (int)HttpStatusCode.NotFound);
            }

            return ServiceResult<StaffResponse>.Ok(ToResponse(staff), "Staff record retrieved successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<StaffResponse>> CreateAsync(CreateStaffRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validationErrors = ValidateRole(request.Role);
            if (validationErrors.Count > 0)
            {
                return ServiceResult<StaffResponse>.Fail("Validation failed.", (int)HttpStatusCode.BadRequest, validationErrors);
            }

            var email = NormalizeEmail(request.Email);
            if (await _staffRepository.EmailExistsAsync(email, null, cancellationToken))
            {
                return ServiceResult<StaffResponse>.Fail("A staff member with this email already exists.", (int)HttpStatusCode.Conflict);
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

            return ServiceResult<StaffResponse>.Ok(ToResponse(staff), "Staff member registered successfully.", (int)HttpStatusCode.Created);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<StaffResponse>> UpdateAsync(int id, UpdateStaffRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<StaffResponse>.Fail("Invalid staff id.", (int)HttpStatusCode.BadRequest);
            }

            var validationErrors = ValidateRole(request.Role);
            if (validationErrors.Count > 0)
            {
                return ServiceResult<StaffResponse>.Fail("Validation failed.", (int)HttpStatusCode.BadRequest, validationErrors);
            }

            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff is null)
            {
                return ServiceResult<StaffResponse>.Fail("Staff member was not found.", (int)HttpStatusCode.NotFound);
            }

            var email = NormalizeEmail(request.Email);
            if (await _staffRepository.EmailExistsAsync(email, id, cancellationToken))
            {
                return ServiceResult<StaffResponse>.Fail("A staff member with this email already exists.", (int)HttpStatusCode.Conflict);
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

            return ServiceResult<StaffResponse>.Ok(ToResponse(staff), "Staff member updated successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<StaffResponse>> UpdateRoleAsync(int id, UpdateStaffRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<StaffResponse>.Fail("Invalid staff id.", (int)HttpStatusCode.BadRequest);
            }

            var validationErrors = ValidateRole(request.Role);
            if (validationErrors.Count > 0)
            {
                return ServiceResult<StaffResponse>.Fail("Validation failed.", (int)HttpStatusCode.BadRequest, validationErrors);
            }

            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff is null)
            {
                return ServiceResult<StaffResponse>.Fail("Staff member was not found.", (int)HttpStatusCode.NotFound);
            }

            staff.Role = request.Role;
            staff.UpdatedAtUtc = DateTime.UtcNow;

            _staffRepository.Update(staff);
            await _staffRepository.SaveChangesAsync(cancellationToken);

            return ServiceResult<StaffResponse>.Ok(ToResponse(staff), "Staff role updated successfully.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return ServiceResult<object>.Fail("Invalid staff id.", (int)HttpStatusCode.BadRequest);
            }

            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff is null)
            {
                return ServiceResult<object>.Fail("Staff member was not found.", (int)HttpStatusCode.NotFound);
            }

            _staffRepository.Delete(staff);
            await _staffRepository.SaveChangesAsync(cancellationToken);

            return ServiceResult<object>.Ok(null, "Staff member deleted successfully.");
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

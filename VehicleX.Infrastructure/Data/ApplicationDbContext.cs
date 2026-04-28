using Microsoft.EntityFrameworkCore;
using VehicleX.Domain.Entities;
using VehicleX.Domain.Enums;

namespace VehicleX.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Staff> Staff => Set<Staff>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("staff");

            entity.HasKey(staff => staff.Id);

            entity.Property(staff => staff.Id)
                .HasColumnName("id");

            entity.Property(staff => staff.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(staff => staff.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(staff => staff.Email)
                .HasColumnName("email")
                .HasMaxLength(180)
                .IsRequired();

            entity.HasIndex(staff => staff.Email)
                .IsUnique();

            entity.Property(staff => staff.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(30);

            entity.Property(staff => staff.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(staff => staff.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(staff => staff.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            entity.Property(staff => staff.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            entity.Property(staff => staff.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");
        });
    }
}

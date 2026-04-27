using Microsoft.EntityFrameworkCore;
using VehicleX.Domain.Entities;

namespace VehicleX.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vendor> Vendors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.ToTable("Vendors");
            entity.HasKey(v => v.Id);

            entity.Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(v => v.ContactPerson)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(v => v.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(v => v.Email)
                .IsUnique();

            entity.Property(v => v.Phone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(v => v.Address)
                .HasMaxLength(500);

            // One to Many: Vendor -> Parts
            
            entity.HasMany(v => v.Parts)
                .WithOne(p => p.Vendor)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

using Microsoft.EntityFrameworkCore;
using VehicleX.Domain.Entities;

namespace VehicleX.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Vendor> Vendors { get; set; } = null!;
    public DbSet<SalesInvoice> SalesInvoices { get; set; }
    public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Vendor configuration
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

        // SalesInvoice to Customer (many invoices per customer)
        modelBuilder.Entity<SalesInvoice>()
            .HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // SalesInvoice to SalesInvoiceItems
        modelBuilder.Entity<SalesInvoiceItem>()
            .HasOne(i => i.SalesInvoice)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SalesInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // PurchaseInvoice to PurchaseInvoiceItems
        modelBuilder.Entity<PurchaseInvoiceItem>()
            .HasOne(i => i.PurchaseInvoice)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.PurchaseInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Decimal precision
        modelBuilder.Entity<SalesInvoice>()
            .Property(s => s.TotalAmount).HasPrecision(18, 2);

        modelBuilder.Entity<SalesInvoiceItem>()
            .Property(s => s.UnitPrice).HasPrecision(18, 2);

        modelBuilder.Entity<PurchaseInvoice>()
            .Property(p => p.TotalAmount).HasPrecision(18, 2);

        modelBuilder.Entity<PurchaseInvoiceItem>()
            .Property(p => p.UnitPrice).HasPrecision(18, 2);
    }
}
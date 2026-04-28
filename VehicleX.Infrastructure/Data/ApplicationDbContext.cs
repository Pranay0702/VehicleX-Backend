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
    public DbSet<SalesInvoice> SalesInvoices { get; set; }
    public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SalesInvoice to Customer (many invoices per customer)
        modelBuilder.Entity<SalesInvoice>()
            .HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // SalesInvoice to SalesInvoiceItems (one invoice has many items)
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
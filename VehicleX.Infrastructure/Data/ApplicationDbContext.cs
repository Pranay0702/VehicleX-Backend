using Microsoft.EntityFrameworkCore;
using VehicleX.Domain.Entities;

namespace VehicleX.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Part> Parts => Set<Part>();

    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();

    public DbSet<SalesItem> SalesItems => Set<SalesItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(customer => customer.Id);

            entity.Property(customer => customer.FullName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(customer => customer.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(customer => customer.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(customer => customer.CreatedAtUtc)
                .IsRequired();

            entity.HasMany(customer => customer.SalesInvoices)
                .WithOne(salesInvoice => salesInvoice.Customer)
                .HasForeignKey(salesInvoice => salesInvoice.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(part => part.Id);

            entity.Property(part => part.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(part => part.PartNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(part => part.PartNumber)
                .IsUnique();

            entity.Property(part => part.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(part => part.StockQuantity)
                .IsRequired();
        });

        modelBuilder.Entity<SalesInvoice>(entity =>
        {
            entity.HasKey(salesInvoice => salesInvoice.Id);

            entity.Property(salesInvoice => salesInvoice.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(salesInvoice => salesInvoice.InvoiceNumber)
                .IsUnique();

            entity.Property(salesInvoice => salesInvoice.CreatedAtUtc)
                .IsRequired();

            entity.Property(salesInvoice => salesInvoice.SubTotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(salesInvoice => salesInvoice.DiscountAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(salesInvoice => salesInvoice.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.HasMany(salesInvoice => salesInvoice.Items)
                .WithOne(item => item.SalesInvoice)
                .HasForeignKey(item => item.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SalesItem>(entity =>
        {
            entity.HasKey(item => item.Id);

            entity.Property(item => item.Quantity)
                .IsRequired();

            entity.Property(item => item.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(item => item.LineTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.HasOne(item => item.Part)
                .WithMany(part => part.SalesItems)
                .HasForeignKey(item => item.PartId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedCreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                FullName = "Saswat Khatry",
                Email = "saswatkc123@gmail.com",
                PhoneNumber = "9808855888",
                CreatedAtUtc = seedCreatedAt
            },
            new Customer
            {
                Id = 2,
                FullName = "Aaryan Jha Sir",
                Email = "AaryanJS@gmail.com",
                PhoneNumber = "+9808855884",
                CreatedAtUtc = seedCreatedAt
            });

        modelBuilder.Entity<Part>().HasData(
            new Part
            {
                Id = 1,
                Name = "Brembo floating disc calipers",
                PartNumber = "BRE-FDC-001",
                UnitPrice = 25000,
                StockQuantity = 40
            },
            new Part
            {
                Id = 2,
                Name = "BMC Air Filter",
                PartNumber = "BMC-AF-2000",
                UnitPrice = 800,
                StockQuantity = 75
            },
            new Part
            {
                Id = 3,
                Name = "LiquiMoly Engine Oil 5W-30",
                PartNumber = "LQML-OIL-530",
                UnitPrice = 1500,
                StockQuantity = 60
            });
    }
}

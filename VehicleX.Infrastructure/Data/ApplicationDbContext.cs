using Microsoft.EntityFrameworkCore;
using VehicleX.Domain.Entities;
using VehicleX.Domain.Enums;

namespace VehicleX.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // -------------------------------------------------------------------------
    // DbSets
    // -------------------------------------------------------------------------

    public DbSet<Staff> Staff => Set<Staff>();

    // Customer domain
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<UnavailablePartRequest> UnavailablePartRequests => Set<UnavailablePartRequest>();
    public DbSet<ServiceReview> ServiceReviews => Set<ServiceReview>();
    public DbSet<CustomerPurchase> CustomerPurchases => Set<CustomerPurchase>();
    public DbSet<CustomerPurchaseItem> CustomerPurchaseItems => Set<CustomerPurchaseItem>();

    // Inventory / purchasing domain
    public DbSet<Vendor> Vendors { get; set; } = null!;
    public DbSet<Part> Parts { get; set; } = null!;
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; } = null!;
    public DbSet<PurchaseInvoiceItem> PurchaseItems { get; set; } = null!;

    // Sales domain
    public DbSet<SalesInvoice> SalesInvoices { get; set; }
    public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -----------------------------------------------------------------
        // Staff
        // -----------------------------------------------------------------
        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("staff");

            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnName("id");

            entity.Property(s => s.FirstName)
                .HasColumnName("first_name").HasMaxLength(80).IsRequired();

            entity.Property(s => s.LastName)
                .HasColumnName("last_name").HasMaxLength(80).IsRequired();

            entity.Property(s => s.Email)
                .HasColumnName("email").HasMaxLength(180).IsRequired();

            entity.HasIndex(s => s.Email).IsUnique();

            entity.Property(s => s.PhoneNumber)
                .HasColumnName("phone_number").HasMaxLength(30);

            entity.Property(s => s.Role)
                .HasColumnName("role").HasConversion<string>().HasMaxLength(40).IsRequired();

            entity.Property(s => s.PasswordHash)
                .HasColumnName("password_hash").HasMaxLength(500).IsRequired();

            entity.Property(s => s.IsActive)
                .HasColumnName("is_active").HasDefaultValue(true).IsRequired();

            entity.Property(s => s.CreatedAtUtc)
                .HasColumnName("created_at_utc").IsRequired();

            entity.Property(s => s.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");
        });
        
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");

            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id");

            entity.Property(c => c.FullName)
                .HasColumnName("full_name").HasMaxLength(160).IsRequired();

            entity.Property(c => c.Email)
                .HasColumnName("email").HasMaxLength(180).IsRequired();

            entity.HasIndex(c => c.Email).IsUnique();

            entity.Property(c => c.PhoneNumber)
                .HasColumnName("phone_number").HasMaxLength(30).IsRequired();

            entity.Property(c => c.Address)
                .HasColumnName("address").HasMaxLength(500);

            entity.Property(c => c.PasswordHash)
                .HasColumnName("password_hash").HasMaxLength(500);

            entity.Property(c => c.IsActive)
                .HasColumnName("is_active").HasDefaultValue(true).IsRequired();

            entity.Property(c => c.CreatedAt)
                .HasColumnName("created_at").IsRequired();

            entity.Property(c => c.UpdatedAt)
                .HasColumnName("updated_at");

            // One-to-Many: Customer → Vehicles
            entity.HasMany(c => c.Vehicles)
                .WithOne(v => v.Customer)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // -----------------------------------------------------------------
        // Appointment
        // -----------------------------------------------------------------
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("appointments");

            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).HasColumnName("id");

            entity.Property(a => a.CustomerId)
                .HasColumnName("customer_id").IsRequired();

            entity.Property(a => a.AppointmentDateUtc)
                .HasColumnName("appointment_date_utc").IsRequired();

            entity.Property(a => a.ServiceType)
                .HasColumnName("service_type").HasMaxLength(120).IsRequired();

            entity.Property(a => a.VehicleMake)
                .HasColumnName("vehicle_make").HasMaxLength(80);

            entity.Property(a => a.VehicleModel)
                .HasColumnName("vehicle_model").HasMaxLength(80);

            entity.Property(a => a.VehicleRegistrationNumber)
                .HasColumnName("vehicle_registration_number").HasMaxLength(30);

            entity.Property(a => a.Notes)
                .HasColumnName("notes").HasMaxLength(1000);

            entity.Property(a => a.Status)
                .HasColumnName("status").HasConversion<string>().HasMaxLength(40).IsRequired();

            entity.Property(a => a.CreatedAtUtc)
                .HasColumnName("created_at_utc").IsRequired();

            entity.Property(a => a.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            entity.HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // -----------------------------------------------------------------
        // UnavailablePartRequest
        // -----------------------------------------------------------------
        modelBuilder.Entity<UnavailablePartRequest>(entity =>
        {
            entity.ToTable("unavailable_part_requests");

            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).HasColumnName("id");

            entity.Property(r => r.CustomerId)
                .HasColumnName("customer_id").IsRequired();

            entity.Property(r => r.PartName)
                .HasColumnName("part_name").HasMaxLength(160).IsRequired();

            entity.Property(r => r.PartNumber)
                .HasColumnName("part_number").HasMaxLength(80);

            entity.Property(r => r.VehicleMake)
                .HasColumnName("vehicle_make").HasMaxLength(80);

            entity.Property(r => r.VehicleModel)
                .HasColumnName("vehicle_model").HasMaxLength(80);

            entity.Property(r => r.Quantity)
                .HasColumnName("quantity").IsRequired();

            entity.ToTable(t => t.HasCheckConstraint(
                "ck_unavailable_part_requests_quantity_positive", "quantity > 0"));

            entity.Property(r => r.Notes)
                .HasColumnName("notes").HasMaxLength(1000);

            entity.Property(r => r.Status)
                .HasColumnName("status").HasConversion<string>().HasMaxLength(40).IsRequired();

            entity.Property(r => r.CreatedAtUtc)
                .HasColumnName("created_at_utc").IsRequired();

            entity.Property(r => r.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            entity.HasOne(r => r.Customer)
                .WithMany(c => c.UnavailablePartRequests)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // -----------------------------------------------------------------
        // ServiceReview 
        // -----------------------------------------------------------------
        modelBuilder.Entity<ServiceReview>(entity =>
        {
            entity.ToTable("service_reviews");

            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).HasColumnName("id");

            entity.Property(r => r.CustomerId)
                .HasColumnName("customer_id").IsRequired();

            entity.Property(r => r.AppointmentId)
                .HasColumnName("appointment_id");

            entity.Property(r => r.Rating)
                .HasColumnName("rating").HasConversion<int>().IsRequired();

            entity.ToTable(t => t.HasCheckConstraint(
                "ck_service_reviews_rating_range", "rating >= 1 AND rating <= 5"));

            entity.Property(r => r.Comment)
                .HasColumnName("comment").HasMaxLength(1200).IsRequired();

            entity.Property(r => r.CreatedAtUtc)
                .HasColumnName("created_at_utc").IsRequired();

            entity.Property(r => r.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            entity.HasOne(r => r.Customer)
                .WithMany(c => c.ServiceReviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Appointment)
                .WithOne(a => a.ServiceReview)
                .HasForeignKey<ServiceReview>(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // A nullable FK can still be unique — partial index excludes NULLs
            entity.HasIndex(r => r.AppointmentId)
                .IsUnique()
                .HasFilter("appointment_id IS NOT NULL");
        });

        // -----------------------------------------------------------------
        // CustomerPurchase  
        // -----------------------------------------------------------------
        modelBuilder.Entity<CustomerPurchase>(entity =>
        {
            entity.ToTable("customer_purchases");

            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id");

            entity.Property(p => p.CustomerId)
                .HasColumnName("customer_id").IsRequired();

            entity.Property(p => p.InvoiceNumber)
                .HasColumnName("invoice_number").HasMaxLength(80).IsRequired();

            entity.HasIndex(p => p.InvoiceNumber).IsUnique();

            entity.Property(p => p.PurchaseDateUtc)
                .HasColumnName("purchase_date_utc").IsRequired();

            entity.Property(p => p.TotalAmount)
                .HasColumnName("total_amount").HasPrecision(12, 2).IsRequired();

            entity.ToTable(t => t.HasCheckConstraint(
                "ck_customer_purchases_total_amount_non_negative", "total_amount >= 0"));

            entity.Property(p => p.Status)
                .HasColumnName("status").HasConversion<string>().HasMaxLength(40).IsRequired();

            entity.Property(p => p.CreatedAtUtc)
                .HasColumnName("created_at_utc").IsRequired();

            entity.Property(p => p.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            entity.HasOne(p => p.Customer)
                .WithMany(c => c.Purchases)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // -----------------------------------------------------------------
        // CustomerPurchaseItem 
        // -----------------------------------------------------------------
        modelBuilder.Entity<CustomerPurchaseItem>(entity =>
        {
            entity.ToTable("customer_purchase_items");

            entity.HasKey(i => i.Id);
            entity.Property(i => i.Id).HasColumnName("id");

            entity.Property(i => i.CustomerPurchaseId)
                .HasColumnName("customer_purchase_id").IsRequired();

            entity.Property(i => i.PartName)
                .HasColumnName("part_name").HasMaxLength(160).IsRequired();

            entity.Property(i => i.PartNumber)
                .HasColumnName("part_number").HasMaxLength(80);

            entity.Property(i => i.Quantity)
                .HasColumnName("quantity").IsRequired();

            entity.Property(i => i.UnitPrice)
                .HasColumnName("unit_price").HasPrecision(12, 2).IsRequired();

            entity.Property(i => i.LineTotal)
                .HasColumnName("line_total").HasPrecision(12, 2).IsRequired();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("ck_customer_purchase_items_quantity_positive",       "quantity > 0");
                t.HasCheckConstraint("ck_customer_purchase_items_unit_price_non_negative", "unit_price >= 0");
                t.HasCheckConstraint("ck_customer_purchase_items_line_total_non_negative", "line_total >= 0");
            });

            entity.Property(i => i.CreatedAtUtc)
                .HasColumnName("created_at_utc").IsRequired();

            entity.HasOne(i => i.CustomerPurchase)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.CustomerPurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // -----------------------------------------------------------------
        // Vendor 
        // -----------------------------------------------------------------
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.ToTable("Vendors");
            entity.HasKey(v => v.Id);

            entity.Property(v => v.Name).IsRequired().HasMaxLength(200);
            entity.Property(v => v.ContactPerson).IsRequired().HasMaxLength(150);
            entity.Property(v => v.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(v => v.Email).IsUnique();
            entity.Property(v => v.Phone).IsRequired().HasMaxLength(20);
            entity.Property(v => v.Address).HasMaxLength(500);

            entity.HasMany(v => v.Parts)
                .WithOne(p => p.Vendor)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // -----------------------------------------------------------------
        // Part 
        // -----------------------------------------------------------------
        modelBuilder.Entity<Part>(entity =>
        {
            entity.ToTable("Parts");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.PartNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(p => p.PartNumber).IsUnique();
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(p => p.StockQuantity).IsRequired().HasDefaultValue(0);

            entity.HasMany(p => p.PurchaseItems)
                .WithOne(pi => pi.Part)
                .HasForeignKey(pi => pi.PartId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // -----------------------------------------------------------------
        // PurchaseInvoice 
        // -----------------------------------------------------------------
        modelBuilder.Entity<PurchaseInvoice>(entity =>
        {
            entity.ToTable("PurchaseInvoices");
            entity.HasKey(pi => pi.Id);

            entity.Property(pi => pi.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(pi => pi.InvoiceNumber).IsUnique();
            entity.Property(pi => pi.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(pi => pi.Notes).HasMaxLength(1000);

            entity.HasMany(pi => pi.Items)
                .WithOne(item => item.PurchaseInvoice)
                .HasForeignKey(item => item.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // -----------------------------------------------------------------
        // PurchaseInvoiceItem 
        // -----------------------------------------------------------------
        modelBuilder.Entity<PurchaseInvoiceItem>(entity =>
        {
            entity.ToTable("PurchaseItems");
            entity.HasKey(pi => pi.Id);

            entity.Property(pi => pi.Quantity).IsRequired();
            entity.Property(pi => pi.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(pi => pi.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
        });

        // -----------------------------------------------------------------
        // SalesInvoice 
        // -----------------------------------------------------------------
        modelBuilder.Entity<SalesInvoice>()
            .HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SalesInvoice>()
            .Property(s => s.TotalAmount).HasPrecision(18, 2);

        // -----------------------------------------------------------------
        // SalesInvoiceItem 
        // -----------------------------------------------------------------
        modelBuilder.Entity<SalesInvoiceItem>()
            .HasOne(i => i.SalesInvoice)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SalesInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SalesInvoiceItem>()
            .Property(s => s.UnitPrice).HasPrecision(18, 2);
        
        modelBuilder.Entity<PurchaseInvoice>()
            .Property(p => p.TotalAmount).HasPrecision(18, 2);

        modelBuilder.Entity<PurchaseInvoiceItem>()
            .Property(p => p.UnitPrice).HasPrecision(18, 2);
    }
}
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
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<UnavailablePartRequest> UnavailablePartRequests => Set<UnavailablePartRequest>();
    public DbSet<ServiceReview> ServiceReviews => Set<ServiceReview>();

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

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");

            entity.HasKey(customer => customer.Id);

            entity.Property(customer => customer.Id)
                .HasColumnName("id");

            entity.Property(customer => customer.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(customer => customer.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(customer => customer.Email)
                .HasColumnName("email")
                .HasMaxLength(180)
                .IsRequired();

            entity.HasIndex(customer => customer.Email)
                .IsUnique();

            entity.Property(customer => customer.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(customer => customer.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            entity.Property(customer => customer.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            entity.Property(customer => customer.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("appointments");

            entity.HasKey(appointment => appointment.Id);

            entity.Property(appointment => appointment.Id)
                .HasColumnName("id");

            entity.Property(appointment => appointment.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            entity.Property(appointment => appointment.AppointmentDateUtc)
                .HasColumnName("appointment_date_utc")
                .IsRequired();

            entity.Property(appointment => appointment.ServiceType)
                .HasColumnName("service_type")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(appointment => appointment.VehicleMake)
                .HasColumnName("vehicle_make")
                .HasMaxLength(80);

            entity.Property(appointment => appointment.VehicleModel)
                .HasColumnName("vehicle_model")
                .HasMaxLength(80);

            entity.Property(appointment => appointment.VehicleRegistrationNumber)
                .HasColumnName("vehicle_registration_number")
                .HasMaxLength(30);

            entity.Property(appointment => appointment.Notes)
                .HasColumnName("notes")
                .HasMaxLength(1000);

            entity.Property(appointment => appointment.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(appointment => appointment.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            entity.Property(appointment => appointment.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            entity.HasOne(appointment => appointment.Customer)
                .WithMany(customer => customer.Appointments)
                .HasForeignKey(appointment => appointment.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UnavailablePartRequest>(entity =>
        {
            entity.ToTable("unavailable_part_requests");

            entity.HasKey(partRequest => partRequest.Id);

            entity.Property(partRequest => partRequest.Id)
                .HasColumnName("id");

            entity.Property(partRequest => partRequest.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            entity.Property(partRequest => partRequest.PartName)
                .HasColumnName("part_name")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(partRequest => partRequest.PartNumber)
                .HasColumnName("part_number")
                .HasMaxLength(80);

            entity.Property(partRequest => partRequest.VehicleMake)
                .HasColumnName("vehicle_make")
                .HasMaxLength(80);

            entity.Property(partRequest => partRequest.VehicleModel)
                .HasColumnName("vehicle_model")
                .HasMaxLength(80);

            entity.Property(partRequest => partRequest.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            entity.ToTable(table => table.HasCheckConstraint("ck_unavailable_part_requests_quantity_positive", "quantity > 0"));

            entity.Property(partRequest => partRequest.Notes)
                .HasColumnName("notes")
                .HasMaxLength(1000);

            entity.Property(partRequest => partRequest.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(partRequest => partRequest.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            entity.Property(partRequest => partRequest.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            entity.HasOne(partRequest => partRequest.Customer)
                .WithMany(customer => customer.UnavailablePartRequests)
                .HasForeignKey(partRequest => partRequest.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ServiceReview>(entity =>
        {
            entity.ToTable("service_reviews");

            entity.HasKey(review => review.Id);

            entity.Property(review => review.Id)
                .HasColumnName("id");

            entity.Property(review => review.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            entity.Property(review => review.AppointmentId)
                .HasColumnName("appointment_id");

            entity.Property(review => review.Rating)
                .HasColumnName("rating")
                .HasConversion<int>()
                .IsRequired();

            entity.ToTable(table => table.HasCheckConstraint("ck_service_reviews_rating_range", "rating >= 1 AND rating <= 5"));

            entity.Property(review => review.Comment)
                .HasColumnName("comment")
                .HasMaxLength(1200)
                .IsRequired();

            entity.Property(review => review.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            entity.Property(review => review.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            entity.HasOne(review => review.Customer)
                .WithMany(customer => customer.ServiceReviews)
                .HasForeignKey(review => review.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(review => review.Appointment)
                .WithOne(appointment => appointment.ServiceReview)
                .HasForeignKey<ServiceReview>(review => review.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(review => review.AppointmentId)
                .IsUnique()
                .HasFilter("appointment_id IS NOT NULL");
        });
    }
}

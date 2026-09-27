namespace MyApp.Appointments.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Appointments.Domain.Entities;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(a => a.SlotId).HasColumnName("slot_id").IsRequired();
        builder.Property(a => a.Status).HasColumnName("status").IsRequired().HasConversion<int>().HasDefaultValue(1);
        builder.Property(a => a.ReservedAt).HasColumnName("reserved_at").IsRequired();
        builder.Property(a => a.ConfirmedAt).HasColumnName("confirmed_at");
        builder.Property(a => a.CancelledAt).HasColumnName("cancelled_at");
        builder.Property(a => a.CancelReason).HasColumnName("cancel_reason");

        builder.Property(a => a.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(a => a.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        builder.Property(a => a.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(a => a.PatientId).HasDatabaseName("ix_appointments_patient_id").HasFilter("is_deleted = false");
        builder.HasIndex(a => a.SlotId).IsUnique().HasDatabaseName("uq_appointments_slot_id");
        builder.HasIndex(a => a.Status).HasDatabaseName("ix_appointments_status").HasFilter("is_deleted = false");
        builder.HasIndex(a => a.ReservedAt).HasDatabaseName("ix_appointments_reserved_at");
    }
}

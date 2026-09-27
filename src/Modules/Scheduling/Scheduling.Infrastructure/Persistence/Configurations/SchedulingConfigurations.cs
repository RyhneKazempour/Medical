namespace MyApp.Scheduling.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Scheduling.Domain.Entities;

public sealed class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
    {
        builder.ToTable("doctor_schedules");

        builder.HasKey(ds => ds.Id);
        builder.Property(ds => ds.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(ds => ds.DoctorHospitalId).HasColumnName("doctor_hospital_id").IsRequired();
        builder.Property(ds => ds.DayOfWeek).HasColumnName("day_of_week").IsRequired();
        builder.Property(ds => ds.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(ds => ds.EndTime).HasColumnName("end_time").IsRequired();
        builder.Property(ds => ds.SlotDurationMinutes).HasColumnName("slot_duration_minutes").IsRequired().HasDefaultValue(30);

        builder.Property(ds => ds.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(ds => ds.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(ds => ds.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(ds => ds.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(ds => ds.UpdatedAt).HasColumnName("updated_at");
        builder.Property(ds => ds.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(ds => ds.DoctorHospitalId).HasDatabaseName("ix_doctor_schedules_doctor_hospital_id").HasFilter("is_deleted = false");
        builder.HasIndex(ds => new { ds.DoctorHospitalId, ds.DayOfWeek }).IsUnique().HasDatabaseName("uq_doctor_schedules_doctor_hospital_day").HasFilter("is_deleted = false");
        builder.HasIndex(ds => ds.IsActive).HasDatabaseName("ix_doctor_schedules_is_active").HasFilter("is_deleted = false");
    }
}

public sealed class DoctorScheduleExceptionConfiguration : IEntityTypeConfiguration<DoctorScheduleException>
{
    public void Configure(EntityTypeBuilder<DoctorScheduleException> builder)
    {
        builder.ToTable("doctor_schedule_exceptions");

        builder.HasKey(dse => dse.Id);
        builder.Property(dse => dse.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(dse => dse.DoctorScheduleId).HasColumnName("doctor_schedule_id").IsRequired();
        builder.Property(dse => dse.ExceptionDate).HasColumnName("exception_date").IsRequired();
        builder.Property(dse => dse.ExceptionType).HasColumnName("exception_type").IsRequired().HasConversion<int>();
        builder.Property(dse => dse.NewStartTime).HasColumnName("new_start_time");
        builder.Property(dse => dse.NewEndTime).HasColumnName("new_end_time");
        builder.Property(dse => dse.Reason).HasColumnName("reason");

        builder.Property(dse => dse.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(dse => dse.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(dse => dse.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(dse => dse.UpdatedAt).HasColumnName("updated_at");
        builder.Property(dse => dse.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasOne(dse => dse.DoctorSchedule).WithMany().HasForeignKey(dse => dse.DoctorScheduleId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(dse => dse.DoctorScheduleId).HasDatabaseName("ix_doctor_schedule_exceptions_doctor_schedule_id").HasFilter("is_deleted = false");
        builder.HasIndex(dse => dse.ExceptionDate).HasDatabaseName("ix_doctor_schedule_exceptions_date").HasFilter("is_deleted = false");
        builder.HasIndex(dse => new { dse.DoctorScheduleId, dse.ExceptionDate }).IsUnique().HasDatabaseName("uq_doctor_schedule_exceptions_schedule_date").HasFilter("is_deleted = false");
    }
}

public sealed class AppointmentSlotConfiguration : IEntityTypeConfiguration<AppointmentSlot>
{
    public void Configure(EntityTypeBuilder<AppointmentSlot> builder)
    {
        builder.ToTable("appointment_slots");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.DoctorScheduleId).HasColumnName("doctor_schedule_id").IsRequired();
        builder.Property(a => a.Date).HasColumnName("date").IsRequired();
        builder.Property(a => a.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(a => a.EndTime).HasColumnName("end_time").IsRequired();
        builder.Property(a => a.Status).HasColumnName("status").IsRequired().HasConversion<int>().HasDefaultValue(1);

        builder.Property(a => a.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(a => a.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        builder.Property(a => a.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasOne(a => a.DoctorSchedule).WithMany().HasForeignKey(a => a.DoctorScheduleId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.DoctorScheduleId).HasDatabaseName("ix_appointment_slots_doctor_schedule_id").HasFilter("is_deleted = false");
        builder.HasIndex(a => a.Date).HasDatabaseName("ix_appointment_slots_date").HasFilter("is_deleted = false");
        builder.HasIndex(a => a.Status).HasDatabaseName("ix_appointment_slots_status").HasFilter("is_deleted = false");
        builder.HasIndex(a => new { a.Date, a.Status }).HasDatabaseName("ix_appointment_slots_date_status").HasFilter("is_deleted = false");
        builder.HasIndex(a => new { a.DoctorScheduleId, a.Date, a.StartTime }).IsUnique().HasDatabaseName("uq_appointment_slots_schedule_date_time").HasFilter("is_deleted = false");
    }
}

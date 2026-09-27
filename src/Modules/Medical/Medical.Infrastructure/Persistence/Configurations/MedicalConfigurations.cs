namespace MyApp.Medical.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Medical.Domain.Entities;

public sealed class HospitalConfiguration : IEntityTypeConfiguration<Hospital>
{
    public void Configure(EntityTypeBuilder<Hospital> builder)
    {
        builder.ToTable("hospitals");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(h => h.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(h => h.Address).HasColumnName("address");
        builder.Property(h => h.Phone).HasColumnName("phone").HasMaxLength(20);
        builder.Property(h => h.Email).HasColumnName("email").HasMaxLength(255);

        builder.Property(h => h.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(h => h.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(h => h.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(h => h.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(h => h.UpdatedAt).HasColumnName("updated_at");
        builder.Property(h => h.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(h => h.Name).IsUnique().HasDatabaseName("uq_hospitals_name").HasFilter("is_deleted = false");
        builder.HasIndex(h => h.IsActive).HasDatabaseName("ix_hospitals_is_active").HasFilter("is_deleted = false");
    }
}

public sealed class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.ToTable("clinics");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.HospitalId).HasColumnName("hospital_id").IsRequired();
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Description).HasColumnName("description");

        builder.Property(c => c.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(c => c.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(c => c.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasOne(c => c.Hospital).WithMany().HasForeignKey(c => c.HospitalId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.HospitalId).HasDatabaseName("ix_clinics_hospital_id").HasFilter("is_deleted = false");
        builder.HasIndex(c => c.IsActive).HasDatabaseName("ix_clinics_is_active").HasFilter("is_deleted = false");
    }
}

public sealed class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.ToTable("specializations");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(s => s.Description).HasColumnName("description");

        builder.Property(s => s.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(s => s.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(s => s.Name).IsUnique().HasDatabaseName("uq_specializations_name").HasFilter("is_deleted = false");
        builder.HasIndex(s => s.IsActive).HasDatabaseName("ix_specializations_is_active").HasFilter("is_deleted = false");
    }
}

public sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctors");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(d => d.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(d => d.LicenseNumber).HasColumnName("license_number").HasMaxLength(50).IsRequired();
        builder.Property(d => d.Bio).HasColumnName("bio");

        builder.Property(d => d.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(d => d.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(d => d.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(d => d.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at");
        builder.Property(d => d.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(d => d.UserId).IsUnique().HasDatabaseName("uq_doctors_user_id");
        builder.HasIndex(d => d.LicenseNumber).IsUnique().HasDatabaseName("uq_doctors_license_number");
        builder.HasIndex(d => d.IsActive).HasDatabaseName("ix_doctors_is_active").HasFilter("is_deleted = false");
    }
}

public sealed class DoctorSpecializationConfiguration : IEntityTypeConfiguration<DoctorSpecialization>
{
    public void Configure(EntityTypeBuilder<DoctorSpecialization> builder)
    {
        builder.ToTable("doctor_specializations");

        builder.HasKey(ds => ds.Id);
        builder.Property(ds => ds.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(ds => ds.DoctorId).HasColumnName("doctor_id").IsRequired();
        builder.Property(ds => ds.SpecializationId).HasColumnName("specialization_id").IsRequired();

        builder.Property(ds => ds.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(ds => ds.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(ds => ds.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(ds => ds.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(ds => ds.UpdatedAt).HasColumnName("updated_at");
        builder.Property(ds => ds.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasOne(ds => ds.Doctor).WithMany().HasForeignKey(ds => ds.DoctorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ds => ds.Specialization).WithMany().HasForeignKey(ds => ds.SpecializationId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ds => ds.DoctorId).HasDatabaseName("ix_doctor_specializations_doctor_id").HasFilter("is_deleted = false");
        builder.HasIndex(ds => ds.SpecializationId).HasDatabaseName("ix_doctor_specializations_specialization_id").HasFilter("is_deleted = false");
        builder.HasIndex(ds => new { ds.DoctorId, ds.SpecializationId }).IsUnique().HasDatabaseName("uq_doctor_specializations_doctor_specialization").HasFilter("is_deleted = false");
    }
}

public sealed class DoctorHospitalConfiguration : IEntityTypeConfiguration<DoctorHospital>
{
    public void Configure(EntityTypeBuilder<DoctorHospital> builder)
    {
        builder.ToTable("doctor_hospitals");

        builder.HasKey(dh => dh.Id);
        builder.Property(dh => dh.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(dh => dh.DoctorSpecializationId).HasColumnName("doctor_specialization_id").IsRequired();
        builder.Property(dh => dh.ClinicId).HasColumnName("clinic_id").IsRequired();
        builder.Property(dh => dh.RoomNumber).HasColumnName("room_number").HasMaxLength(20);
        builder.Property(dh => dh.StartContractDate).HasColumnName("start_contract_date").IsRequired();
        builder.Property(dh => dh.EndContractDate).HasColumnName("end_contract_date");

        builder.Property(dh => dh.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(dh => dh.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(dh => dh.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(dh => dh.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(dh => dh.UpdatedAt).HasColumnName("updated_at");
        builder.Property(dh => dh.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasOne(dh => dh.DoctorSpecialization).WithMany().HasForeignKey(dh => dh.DoctorSpecializationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(dh => dh.Clinic).WithMany().HasForeignKey(dh => dh.ClinicId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(dh => dh.DoctorSpecializationId).HasDatabaseName("ix_doctor_hospitals_doctor_specialization_id").HasFilter("is_deleted = false");
        builder.HasIndex(dh => dh.ClinicId).HasDatabaseName("ix_doctor_hospitals_clinic_id").HasFilter("is_deleted = false");
        builder.HasIndex(dh => new { dh.DoctorSpecializationId, dh.ClinicId }).IsUnique().HasDatabaseName("uq_doctor_hospitals_doctor_spec_clinic").HasFilter("is_deleted = false");
        builder.HasIndex(dh => dh.IsActive).HasDatabaseName("ix_doctor_hospitals_is_active").HasFilter("is_deleted = false");
    }
}

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(p => p.DateOfBirth).HasColumnName("date_of_birth");
        builder.Property(p => p.Gender).HasColumnName("gender").HasMaxLength(20);
        builder.Property(p => p.BloodType).HasColumnName("blood_type").HasMaxLength(5);
        builder.Property(p => p.EmergencyContactName).HasColumnName("emergency_contact_name").HasMaxLength(200);
        builder.Property(p => p.EmergencyContactPhone).HasColumnName("emergency_contact_phone").HasMaxLength(20);
        builder.Property(p => p.Address).HasColumnName("address");

        builder.Property(p => p.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(p => p.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(p => p.UserId).IsUnique().HasDatabaseName("uq_patients_user_id");
    }
}

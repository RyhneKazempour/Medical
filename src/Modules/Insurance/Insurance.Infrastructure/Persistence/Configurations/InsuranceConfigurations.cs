namespace MyApp.Insurance.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Insurance.Domain.Entities;

public sealed class InsuranceConfiguration : IEntityTypeConfiguration<Insurance>
{
    public void Configure(EntityTypeBuilder<Insurance> builder)
    {
        builder.ToTable("insurances");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(i => i.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(i => i.ContactPhone).HasColumnName("contact_phone").HasMaxLength(20);
        builder.Property(i => i.ContactEmail).HasColumnName("contact_email").HasMaxLength(255);
        builder.Property(i => i.Address).HasColumnName("address");

        builder.Property(i => i.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(i => i.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(i => i.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(i => i.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");
        builder.Property(i => i.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(i => i.Code).IsUnique().HasDatabaseName("uq_insurances_code").HasFilter("is_deleted = false");
        builder.HasIndex(i => i.IsActive).HasDatabaseName("ix_insurances_is_active").HasFilter("is_deleted = false");
    }
}

public sealed class PatientInsuranceConfiguration : IEntityTypeConfiguration<PatientInsurance>
{
    public void Configure(EntityTypeBuilder<PatientInsurance> builder)
    {
        builder.ToTable("patient_insurances");

        builder.HasKey(pi => pi.Id);
        builder.Property(pi => pi.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(pi => pi.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(pi => pi.InsuranceId).HasColumnName("insurance_id").IsRequired();
        builder.Property(pi => pi.PolicyNumber).HasColumnName("policy_number").HasMaxLength(100).IsRequired();
        builder.Property(pi => pi.GroupNumber).HasColumnName("group_number").HasMaxLength(100);
        builder.Property(pi => pi.IsPrimary).HasColumnName("is_primary").IsRequired().HasDefaultValue(false);
        builder.Property(pi => pi.ValidFrom).HasColumnName("valid_from").IsRequired();
        builder.Property(pi => pi.ValidUntil).HasColumnName("valid_until");

        builder.Property(pi => pi.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(pi => pi.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(pi => pi.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(pi => pi.UpdatedAt).HasColumnName("updated_at");
        builder.Property(pi => pi.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasOne(pi => pi.Insurance).WithMany().HasForeignKey(pi => pi.InsuranceId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pi => pi.PatientId).HasDatabaseName("ix_patient_insurances_patient_id").HasFilter("is_deleted = false");
        builder.HasIndex(pi => pi.InsuranceId).HasDatabaseName("ix_patient_insurances_insurance_id").HasFilter("is_deleted = false");
        builder.HasIndex(pi => new { pi.PatientId, pi.InsuranceId }).IsUnique().HasDatabaseName("uq_patient_insurances_patient_insurance").HasFilter("is_deleted = false");

        // Partial unique index for primary insurance will be created via migration
    }
}

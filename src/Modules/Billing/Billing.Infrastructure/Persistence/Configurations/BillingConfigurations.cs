namespace MyApp.Billing.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApp.Billing.Domain.Entities;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.AppointmentId).HasColumnName("appointment_id").IsRequired();
        builder.Property(p => p.AmountCents).HasColumnName("amount_cents").IsRequired();
        builder.Property(p => p.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired().HasDefaultValue("USD");
        builder.Property(p => p.TransactionNumber).HasColumnName("transaction_number").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Status).HasColumnName("status").IsRequired().HasConversion<int>().HasDefaultValue(PaymentStatus.Pending);
        builder.Property(p => p.PaymentMethod).HasColumnName("payment_method").IsRequired().HasConversion<int>();
        builder.Property(p => p.PaidAt).HasColumnName("paid_at");
        builder.Property(p => p.InsuranceId).HasColumnName("insurance_id");

        builder.Property(p => p.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(p => p.CreatedUserId).HasColumnName("created_user_id");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.UpdatedUserId).HasColumnName("updated_user_id");

        builder.HasIndex(p => p.AppointmentId).HasDatabaseName("ix_payments_appointment_id").HasFilter("is_deleted = false");
        builder.HasIndex(p => p.TransactionNumber).IsUnique().HasDatabaseName("uq_payments_transaction_number").HasFilter("is_deleted = false");
        builder.HasIndex(p => p.Status).HasDatabaseName("ix_payments_status").HasFilter("is_deleted = false");
        builder.HasIndex(p => p.InsuranceId).HasDatabaseName("ix_payments_insurance_id").HasFilter("is_deleted = false");
    }
}

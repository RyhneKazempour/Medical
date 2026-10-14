namespace Billing.Infrastructure.Persistence.DbContext;

using Billing.Domain.Entities;
using Billing.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Abstractions;
using Shared.Infrastructure.Persistence;

public sealed class BillingDbContext : DbContext, IUnitOfWork
{
    public DbSet<Payment> Payments => Set<Payment>();

    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());

        modelBuilder.ApplySoftDeleteQueryFilters();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}

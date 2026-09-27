namespace MyApp.Billing.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using MyApp.Billing.Domain.Entities;
using MyApp.Billing.Infrastructure.Persistence.Configurations;
using MyApp.Shared.Application.Abstractions;

public sealed class BillingDbContext : DbContext, IUnitOfWork
{
    public DbSet<Payment> Payments => Set<Payment>();

    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());

        modelBuilder.Entity<Payment>().HasQueryFilter(p => !p.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}

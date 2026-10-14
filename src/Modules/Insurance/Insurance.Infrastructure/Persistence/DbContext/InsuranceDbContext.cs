namespace Insurance.Infrastructure.Persistence.DbContext;

using Insurance.Domain.Entities;
using Insurance.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Abstractions;
using Shared.Infrastructure.Persistence;

public sealed class InsuranceDbContext : DbContext, IUnitOfWork
{
    public DbSet<InsuranceInfo> Insurances => Set<InsuranceInfo>();
    public DbSet<PatientInsurance> PatientInsurances => Set<PatientInsurance>();

    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InsuranceConfiguration());
        modelBuilder.ApplyConfiguration(new PatientInsuranceConfiguration());

        modelBuilder.ApplySoftDeleteQueryFilters();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}

namespace MyApp.Insurance.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using MyApp.Insurance.Domain.Entities;
using MyApp.Insurance.Infrastructure.Persistence.Configurations;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Infrastructure.Persistence;

public sealed class InsuranceDbContext : DbContext, IUnitOfWork
{
    public DbSet<Insurance> Insurances => Set<Insurance>();
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

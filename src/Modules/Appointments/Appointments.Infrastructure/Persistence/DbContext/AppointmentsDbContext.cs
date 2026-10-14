namespace Appointments.Infrastructure.Persistence.DbContext;

using Appointments.Domain.Entities;
using Appointments.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Abstractions;
using Shared.Infrastructure.Persistence;

public sealed class AppointmentsDbContext : DbContext, IUnitOfWork
{
    public DbSet<Appointment> Appointments => Set<Appointment>();

    public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());

        modelBuilder.ApplySoftDeleteQueryFilters();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}

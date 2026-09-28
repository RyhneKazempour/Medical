namespace MyApp.Appointments.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using MyApp.Appointments.Domain.Entities;
using MyApp.Appointments.Infrastructure.Persistence.Configurations;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Infrastructure.Persistence;

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

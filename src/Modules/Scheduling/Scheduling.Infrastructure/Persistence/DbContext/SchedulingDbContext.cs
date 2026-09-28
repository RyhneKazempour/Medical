namespace MyApp.Scheduling.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using MyApp.Scheduling.Domain.Entities;
using MyApp.Scheduling.Infrastructure.Persistence.Configurations;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Infrastructure.Persistence;

public sealed class SchedulingDbContext : DbContext, IUnitOfWork
{
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
    public DbSet<DoctorScheduleException> DoctorScheduleExceptions => Set<DoctorScheduleException>();
    public DbSet<AppointmentSlot> AppointmentSlots => Set<AppointmentSlot>();

    public SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DoctorScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorScheduleExceptionConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentSlotConfiguration());

        modelBuilder.ApplySoftDeleteQueryFilters();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}

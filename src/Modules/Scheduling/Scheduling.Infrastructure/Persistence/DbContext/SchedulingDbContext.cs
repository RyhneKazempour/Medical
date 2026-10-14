namespace Scheduling.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Entities;
using Scheduling.Infrastructure.Persistence.Configurations;
using Shared.Application.Abstractions;
using Shared.Infrastructure.Persistence;

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

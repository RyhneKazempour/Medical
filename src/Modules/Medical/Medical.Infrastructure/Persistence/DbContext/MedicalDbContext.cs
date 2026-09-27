namespace MyApp.Medical.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using MyApp.Medical.Domain.Entities;
using MyApp.Medical.Infrastructure.Persistence.Configurations;
using MyApp.Shared.Application.Abstractions;

public sealed class MedicalDbContext : DbContext, IUnitOfWork
{
    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorSpecialization> DoctorSpecializations => Set<DoctorSpecialization>();
    public DbSet<DoctorHospital> DoctorHospitals => Set<DoctorHospital>();
    public DbSet<Patient> Patients => Set<Patient>();

    public MedicalDbContext(DbContextOptions<MedicalDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new HospitalConfiguration());
        modelBuilder.ApplyConfiguration(new ClinicConfiguration());
        modelBuilder.ApplyConfiguration(new SpecializationConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorSpecializationConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorHospitalConfiguration());
        modelBuilder.ApplyConfiguration(new PatientConfiguration());

        // Global query filters for soft delete
        modelBuilder.Entity<Hospital>().HasQueryFilter(h => !h.IsDeleted);
        modelBuilder.Entity<Clinic>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Specialization>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<Doctor>().HasQueryFilter(d => !d.IsDeleted);
        modelBuilder.Entity<DoctorSpecialization>().HasQueryFilter(ds => !ds.IsDeleted);
        modelBuilder.Entity<DoctorHospital>().HasQueryFilter(dh => !dh.IsDeleted);
        modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}

namespace Medical.Infrastructure.Persistence.DbContext;

using Medical.Domain.Entities;
using Medical.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Abstractions;
using Shared.Infrastructure.Persistence;

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

        modelBuilder.ApplySoftDeleteQueryFilters();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}

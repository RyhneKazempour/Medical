namespace MyApp.Medical.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Medical.Application.Abstractions;
using MyApp.Medical.Infrastructure.Persistence.DbContext;
using MyApp.Medical.Infrastructure.Persistence.Repositories;
using MyApp.Shared.Application.Abstractions;

public static class DependencyInjection
{
    public static IServiceCollection AddMedicalInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<MedicalDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(3);
            });
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MedicalDbContext>());

        services.AddScoped<IHospitalRepository, HospitalRepository>();
        services.AddScoped<IClinicRepository, ClinicRepository>();
        services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IDoctorSpecializationRepository, DoctorSpecializationRepository>();
        services.AddScoped<IDoctorHospitalRepository, DoctorHospitalRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();

        return services;
    }
}

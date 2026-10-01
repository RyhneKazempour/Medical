namespace MyApp.Scheduling.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Scheduling.Application.Abstractions;
using MyApp.Scheduling.Infrastructure.Persistence.DbContext;
using MyApp.Scheduling.Infrastructure.Persistence.Repositories;
using MyApp.Shared.Application.Abstractions;

public static class DependencyInjection
{
    public static IServiceCollection AddSchedulingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<SchedulingDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(3);
            });
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SchedulingDbContext>());

        services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
        services.AddScoped<IDoctorScheduleExceptionRepository, DoctorScheduleExceptionRepository>();
        services.AddScoped<IAppointmentSlotRepository, AppointmentSlotRepository>();

        return services;
    }
}

namespace MyApp.Appointments.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Appointments.Application.Abstractions;
using MyApp.Appointments.Infrastructure.Persistence.DbContext;
using MyApp.Appointments.Infrastructure.Persistence.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddAppointmentsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppointmentsDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(3);
            });
        });

        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        return services;
    }
}

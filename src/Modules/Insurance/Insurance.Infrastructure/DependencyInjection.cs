namespace Insurance.Infrastructure;

using Insurance.Application.Abstractions;
using Insurance.Infrastructure.Persistence.DbContext;
using Insurance.Infrastructure.Persistence.Repositories;
using InsuranceInfo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Abstractions;

public static class DependencyInjection
{
    public static IServiceCollection AddInsuranceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<InsuranceDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(3);
            });
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InsuranceDbContext>());

        services.AddScoped<IInsuranceRepository, InsuranceRepository>();
        services.AddScoped<IPatientInsuranceRepository, PatientInsuranceRepository>();

        return services;
    }
}

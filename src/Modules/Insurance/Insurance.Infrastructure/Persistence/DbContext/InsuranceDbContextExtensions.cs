namespace MyApp.Insurance.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class InsuranceDbContextExtensions
{
    public static void ApplyInsuranceMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<InsuranceDbContext>();

        dbContext.Database.Migrate();
    }
}

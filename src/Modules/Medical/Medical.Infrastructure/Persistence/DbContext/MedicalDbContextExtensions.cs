namespace MyApp.Medical.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class MedicalDbContextExtensions
{
    public static void ApplyMedicalMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<MedicalDbContext>();

        dbContext.Database.Migrate();
    }
}

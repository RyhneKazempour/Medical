namespace MyApp.Scheduling.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class SchedulingDbContextExtensions
{
    public static void ApplySchedulingMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<SchedulingDbContext>();

        dbContext.Database.Migrate();
    }
}

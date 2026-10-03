namespace MyApp.Identity.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class IdentityDbContextExtensions
{
    public static void ApplyIdentityMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<IdentityDbContext>();

        dbContext.Database.Migrate();
    }
}

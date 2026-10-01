namespace MyApp.Billing.Infrastructure.Persistence.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class BillingDbContextExtensions
{
    public static void ApplyBillingMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<BillingDbContext>();

        dbContext.Database.Migrate();
    }
}

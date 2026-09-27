namespace MyApp.Identity.Application;

using Microsoft.Extensions.DependencyInjection;
using MyApp.Identity.Application.Abstractions;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}

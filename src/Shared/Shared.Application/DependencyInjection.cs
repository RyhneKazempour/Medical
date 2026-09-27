namespace MyApp.Shared.Application;

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Shared.Application.Abstractions;
using MyApp.Shared.Application.Behaviors;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public DateTimeOffset Now => DateTimeOffset.Now;
}

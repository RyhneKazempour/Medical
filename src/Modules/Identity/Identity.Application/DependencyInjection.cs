namespace MyApp.Identity.Application;

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Identity.Application.Abstractions;
using MyApp.Shared.Application.Behaviors;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}

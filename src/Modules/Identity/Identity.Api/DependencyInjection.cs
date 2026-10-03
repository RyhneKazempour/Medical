namespace MyApp.Identity.Api;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(this IServiceCollection services)
    {
        return services;
    }
}
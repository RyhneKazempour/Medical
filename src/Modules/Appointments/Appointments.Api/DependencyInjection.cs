namespace MyApp.Appointments.Api;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAppointmentsApi(this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapAppointmentsApi(this IEndpointRouteBuilder app)
    {
        return app;
    }
}

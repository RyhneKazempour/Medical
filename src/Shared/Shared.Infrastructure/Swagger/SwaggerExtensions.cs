namespace MyApp.Shared.Infrastructure.Swagger;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

public static class SwaggerExtensions
{
    private const string ApiVersion = "v1";
    private const string ApiTitle = "Medical Appointment API";
    private const string SecuritySchemeName = "Bearer";

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(TransformOpenApiDocument);
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        var isEnabled = app.Environment.IsDevelopment()
            || app.Configuration.GetValue<bool>("Swagger:Enabled");

        if (!isEnabled)
        {
            return app;
        }

        app.MapOpenApi("/openapi/{documentName}");

        app.MapScalarApiReference(options =>
        {
            options.WithTitle(ApiTitle)
                   .WithOpenApiRoutePattern($"/openapi/{ApiVersion}")
                   .WithEndpointPrefix($"/scalar/{{documentName}}");
        });

        return app;
    }

    private static async Task TransformOpenApiDocument(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = ApiTitle,
            Version = ApiVersion,
            Description = "A modular monolith API for managing medical appointments, clinics, doctors, patients, scheduling, insurance and billing.",
            Contact = new OpenApiContact
            {
                Name = "Medical Appointment Team",
                Email = "dev@medical.example"
            },
            License = new OpenApiLicense { Name = "MIT" }
        };

        document.Servers ??= new List<OpenApiServer>();
        document.Servers.Add(new OpenApiServer
        {
            Url = "/",
            Description = "Local development server"
        });

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SecuritySchemeName] = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT authorization header using the Bearer scheme. Enter 'Bearer {token}' to authenticate."
        };

        document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = SecuritySchemeName
                    }
                },
                Array.Empty<string>()
            }
        });

        await Task.CompletedTask;
    }
}
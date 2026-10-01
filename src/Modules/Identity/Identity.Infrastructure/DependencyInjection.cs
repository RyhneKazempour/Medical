namespace MyApp.Identity.Infrastructure;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MyApp.Identity.Application.Abstractions;
using MyApp.Identity.Infrastructure.Authentication;
using MyApp.Identity.Infrastructure.Authorization;
using MyApp.Identity.Infrastructure.Persistence.DbContext;
using MyApp.Identity.Infrastructure.Persistence.Repositories;
using MyApp.Shared.Application.Abstractions;
using System.Text;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(3);
            });
        });

        // Register IUnitOfWork with IdentityDbContext
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateOnStart()
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Jwt:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Jwt:Audience is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SecretKey), "Jwt:SecretKey is required.")
            .Validate(options => options.SecretKey?.Length >= 32, "Jwt:SecretKey must be at least 32 characters.")
            .Validate(options => options.AccessTokenExpirationMinutes > 0, "Jwt:AccessTokenExpirationMinutes must be greater than 0.")
            .Validate(options => options.RefreshTokenExpirationDays > 0, "Jwt:RefreshTokenExpirationDays must be greater than 0.");

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IAccessTokenGenerator, AccessTokenGenerator>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!)),
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
        });

        services.AddScoped<IAuthorizationHandler, ScopeAuthorizationHandler>();

        return services;
    }
}

using Microsoft.AspNetCore.RateLimiting;
using Medical.Application;
using Medical.Api;
using Medical.Infrastructure;
using Medical.Infrastructure.Persistence.DbContext;
using Identity.Api;
using Identity.Infrastructure.Persistence.DbContext;
using Identity.Infrastructure;
using Scheduling.Infrastructure.Persistence.DbContext;
using Scheduling.Infrastructure;
using Identity.Application;
using Scheduling.Application;
using Shared.Infrastructure;
using Shared.Infrastructure.Observability;
using Scheduling.Api;
using Appointments.Application;
using Appointments.Infrastructure;
using Appointments.Infrastructure.Persistence.DbContext;
using Appointments.Api;
using Insurance.Application;
using Insurance.Infrastructure.Persistence.DbContext;
using Insurance.Infrastructure;
using Insurance.Api;
using Billing.Infrastructure;
using Billing.Infrastructure.Persistence.DbContext;
using Billing.Api;
using Billing.Application;
using Shared.Api.Resilience;
using Shared.Api.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApi();

builder.Services.AddRateLimitingConfiguration();
builder.Services.AddResilienceConfiguration(builder.Configuration);

builder.Services.AddMedicalApplication();
builder.Services.AddMedicalInfrastructure(builder.Configuration);
builder.Services.AddMedicalApi();

builder.Services.AddSchedulingApplication();
builder.Services.AddSchedulingInfrastructure(builder.Configuration);
builder.Services.AddSchedulingApi();

builder.Services.AddAppointmentsApplication();
builder.Services.AddAppointmentsInfrastructure(builder.Configuration);
builder.Services.AddAppointmentsApi();

builder.Services.AddInsuranceApplication();
builder.Services.AddInsuranceInfrastructure(builder.Configuration);
builder.Services.AddInsuranceApi();

builder.Services.AddBillingApplication();
builder.Services.AddBillingInfrastructure(builder.Configuration);
builder.Services.AddBillingApi();

builder.Services.AddApplicationObservability(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Services.ApplyIdentityMigrations();
    app.Services.ApplyMedicalMigrations();
    app.Services.ApplySchedulingMigrations();
    app.Services.ApplyAppointmentsMigrations();
    app.Services.ApplyInsuranceMigrations();
    app.Services.ApplyBillingMigrations();
}


app.UseSharedInfrastructure();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapIdentityApi();
app.MapMedicalApi();
app.MapSchedulingApi();
app.MapAppointmentsApi();
app.MapInsuranceApi();
app.MapBillingApi();

app.Run();

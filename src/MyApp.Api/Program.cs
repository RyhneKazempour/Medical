using MyApp.Appointments.Api;
using MyApp.Appointments.Application;
using MyApp.Appointments.Infrastructure;
using MyApp.Billing.Api;
using MyApp.Billing.Application;
using MyApp.Billing.Infrastructure;
using MyApp.Identity.Api;
using MyApp.Identity.Application;
using MyApp.Identity.Infrastructure;
using MyApp.Identity.Infrastructure.Persistence.DbContext;
using MyApp.Insurance.Infrastructure.Persistence.DbContext;
using MyApp.Medical.Infrastructure.Persistence.DbContext;
using MyApp.Scheduling.Infrastructure.Persistence.DbContext;
using MyApp.Appointments.Infrastructure.Persistence.DbContext;
using MyApp.Billing.Infrastructure.Persistence.DbContext;
using MyApp.Insurance.Api;
using MyApp.Insurance.Application;
using MyApp.Insurance.Infrastructure;
using MyApp.Medical.Api;
using MyApp.Medical.Application;
using MyApp.Medical.Infrastructure;
using MyApp.Scheduling.Api;
using MyApp.Scheduling.Application;
using MyApp.Scheduling.Infrastructure;
using MyApp.Shared.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using MyApp.Shared.Api.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApi();

builder.Services.AddRateLimitingConfiguration();

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

app.MapIdentityApi();
app.MapMedicalApi();
app.MapSchedulingApi();
app.MapAppointmentsApi();
app.MapInsuranceApi();
app.MapBillingApi();

app.Run();

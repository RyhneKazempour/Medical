using MyApp.Shared.Infrastructure;
using MyApp.Identity.Application;
using MyApp.Identity.Infrastructure;
using MyApp.Identity.Api;
using MyApp.Medical.Application;
using MyApp.Medical.Infrastructure;
using MyApp.Medical.Api;
using MyApp.Scheduling.Application;
using MyApp.Scheduling.Infrastructure;
using MyApp.Scheduling.Api;
using MyApp.Appointments.Application;
using MyApp.Appointments.Infrastructure;
using MyApp.Appointments.Api;
using MyApp.Insurance.Application;
using MyApp.Insurance.Infrastructure;
using MyApp.Insurance.Api;
using MyApp.Billing.Application;
using MyApp.Billing.Infrastructure;
using MyApp.Billing.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApi();

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

app.UseSharedInfrastructure();
app.MapIdentityApi();
app.MapMedicalApi();
app.MapSchedulingApi();
app.MapAppointmentsApi();
app.MapInsuranceApi();
app.MapBillingApi();

app.Run();

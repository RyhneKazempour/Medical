# AGENTS.md - Medical Appointment API

This file contains compact, repo-specific guidance for Kilo sessions. Every line answers: "Would an agent likely miss this without help?" If not, it's left out.

## Directory Structure

- `src/MyApp.Api/` - Entry point (Program.cs). Do not move or rename.
- `src/Modules/` - 6 feature modules: Identity, Medical, Scheduling, Appointments, Insurance, Billing
  - Each module has: Domain, Application, Infrastructure, Api sub-folders
  - Modules are independent: no direct references to another module's Infrastructure/DbContext
- `tests/` - Unit tests (xunit + Moq) and Integration tests (Testcontainers + PostgreSQL)
- `Shared/` - Cross-cutting concerns (Domain, Application, Infrastructure)

## Build / Test / Lint Commands

| Command | Purpose |
|---------|---------|
| `dotnet build` | Build the full solution |
| `dotnet build -c Release` | Release build |
| `dotnet test --filter "FullyQualifiedName~MyApp.UnitTests"` | Run unit tests only |
| `dotnet test --filter "FullyQualifiedName~MyApp.IntegrationTests"` | Run integration tests (requires Docker) |
| `docker compose up -d` | Start dev dependencies (Postgres, Prometheus, Grafana) |
| `docker compose down` | Stop dev dependencies |

## Module Registration Pattern (CRITICAL)

In `src/MyApp.Api\Program.cs`, modules are registered in this exact order:

```csharp
builder.Services.AddSharedInfrastructure(builder.Configuration);  // First
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
```

Each module provides 3 extension methods:
- `AddXxxApplication()` - registers MediatR, validators, behaviors
- `AddXxxInfrastructure()` - registers DbContext, repositories, JWT options
- `AddXxxApi()` - may register endpoints (some modules are no-ops, e.g. `AddMedicalApi` returns `services` unchanged)

## Key Conventions

- **Implicit usings**: Enabled globally - no `using` boilerplate needed for common types
- **Treat warnings as errors**: Enabled - warnings break build
- **Nullable reference types**: Enabled (`<Nullable>enable</Nullable>`)
- **Central package version management**: `Directory.Packages.props` controls all package versions
- **NoWarn**: `1591;NU1902;NU1603` (suppress missing XML doc, transitive pinned versions, nullable warnings)

## Environment Configuration

- `appsettings.json` - Production settings (Swagger disabled, `OtlpEnabled: false`)
- `appsettings.Development.json` - Development settings (Swagger enabled)
- Connection string: `DefaultConnection` in both files uses PostgreSQL on localhost:5432
- JWT secrets: `__REPLACE_WITH_A_STRONG_SECRET_AT_LEAST_32_CHARS__` - **do not commit real secrets**
- Set environment variables or update `appsettings.json` for non-local deployments

## OpenTelemetry

- Instrumentation added via `Shared.Infrastructure.OpenTelemetryExtensions.AddApplicationObservability()`
- Metrics: ASP.NET Core, Runtime, Prometheus exporter
- Tracing: ASP.NET Core, HttpClient, EF Core
- Console exporter in Development; OTLP disabled in Production by default
- Prometheus endpoint: `/metrics` (via `UseOpenTelemetryPrometheusScrapingEndpoint()`)

## Rate Limiting & Resilience

- Rate limiting configured via `builder.Services.AddRateLimitingConfiguration()` 
- Resilience (Polly) configured via `builder.Services.AddResilienceConfiguration(builder.Configuration)`
- Both set up in `Program.cs` under `Shared Infrastructure` registration
- Default policies: Payment service timeout 3s, 2 retries with jitter; Notification timeout 5s, 3 retries
- Circuit breaker thresholds: Failure ratio 0.5, min throughput 10, sampling 30s

## Database & Migrations

- EF Core with Npgsql (PostgreSQL)
- Migration assembly: Each module's Infrastructure project specifies its own migrations assembly
- **Critical**: On first run in Development environment, Program.cs applies all migrations:
  ```csharp
  if (app.Environment.IsDevelopment())
  {
      app.Services.ApplyIdentityMigrations();
      app.Services.ApplyMedicalMigrations();
      app.Services.ApplySchedulingMigrations();
      app.Services.ApplyAppointmentsMigrations();
      app.Services.ApplyInsuranceMigrations();
      app.Services.ApplyBillingMigrations();
  }
  ```
- Do not add migration code manually in Production without running migrations first
- Use `dotnet ef migrations` tools from the respective Infrastructure project

## Testing

- **Unit tests**: xunit + FluentAssertions + Moq. Project: `tests\MyApp.UnitTests`
  - References module Domains and Applications (no Infrastructure)
  - Run: `dotnet test --filter "FullyQualifiedName~MyApp.UnitTests"`
- **Integration tests**: Testcontainers + PostgreSQL. Project: `tests\MyApp.IntegrationTests`
  - Requires Docker: `docker compose up -d` first
  - References full API + all Infrastructure projects
  - Run: `dotnet test --filter "FullyQualifiedName~MyApp.IntegrationTests"`
- **Test order matters**: Integration tests share a database - do not run in parallel without isolation
- Tests use `MyApp.Api` factory via `Microsoft.AspNetCore.Mvc.Testing`

## Module Ownership & Boundaries

| Module | Core Responsibility | Key Tables/Entities |
|--------|--------------------|---------------------|
| **Identity** | Auth/Authorization | users, roles, permissions |
| **Medical** | Healthcare providers/patients | hospitals, clinics, doctors, specializations, patients |
| **Scheduling** | Doctor availability | doctor_schedules, appointment_slots |
| **Appointments** | Patient booking | appointments, linked to scheduling slots |
| **Insurance** | Coverage management | insurances, patient_insurances |
| **Billing** | Payment processing | payments (linked to appointments) |

**Cross-module communication rules** (no direct Infrastructure references):
- Appointments → Medical (Patients) via `IPatientRepository`
- Appointments → Scheduling via `IAppointmentSlotRepository`
- Appointments → Insurance via `IPatientInsuranceRepository`
- Billing → Appointments via `IAppointmentRepository`
- Scheduling → Medical (DoctorHospital) via `IDoctorHospitalRepository`
- Medical (Patients) → Identity (Users) via `IUserRepository`

## Common Gotchas

1. **`clinic` → `clinics`**: Table is pluralized. The singular `client` name in Architecture Analysis was corrected to `clinics` to match PostgreSQL conventions. The codebase uses `Clinic` entity class but the DB table is `clinics`.

2. **Migration application order**: Program.cs applies 6 migration groups in a fixed order. If adding a new module, add its `ApplyXxxMigrations()` call here.

3. **JwtOptions validation**: `AddXxxInfrastructure()` validates JWT config on startup. If `SecretKey < 32 chars`, app fails to start. The placeholder `__REPLACE_WITH_A_STRONG_SECRET_AT_LEAST_32_CHARS__` must be replaced.

4. **Rate limiter before auth**: In `Program.cs`, `UseRateLimiter()` is called BEFORE `UseAuthentication()` and `UseAuthorization()`. This means rate limiting applies to unauthenticated requests only.

5. **OpenTelemetry before prometheus**: `UseOpenTelemetryPrometheusScrapingEndpoint()` must be called after `AddApplicationObservability()` in the pipeline.

6. **NoSwagger in Production**: Swagger is gated by `app.Environment.IsDevelopment()` or `Swagger:Enabled` app setting. Do not enable Swagger in production without reviewing security implications.

7. **Shared.Infrastructure first**: `AddSharedInfrastructure()` must be called before module-specific registrations. It sets up HealthChecks, Swagger, CurrentUser, and ProblemDetails.
# AGENTS.md - Medical Appointment API

## Critical Setup
1. Run `docker compose up -d` before integration tests (starts PostgreSQL)
2. In Development, migrations apply automatically on startup
3. JWT secret must be ≥32 chars: `__REPLACE_WITH_A_STRONG_SECRET_AT_LEAST_32_CHARS__`
4. Swagger enabled only in Development via `appsettings.Development.json`

## Module Registration Order (Program.cs)
Modules must be registered in this exact order:
1. SharedInfrastructure
2. Identity (Application → Infrastructure → Api)
3. RateLimitingConfiguration
4. ResilienceConfiguration
5. Medical (Application → Infrastructure → Api)
6. Scheduling (Application → Infrastructure → Api)
7. Appointments (Application → Infrastructure → Api)
8. Insurance (Application → Infrastructure → Api)
9. Billing (Application → Infrastructure → Api)
10. ApplicationObservability

## Key Conventions
- Central package management via `Directory.Packages.props`
- Treat warnings as errors (`TreatWarningsAsErrors>true</TreatWarningsAsErrors>`)
- Nullable reference types enabled
- Implicit usings globally
- NoWarn: 1591;NU1902;NU1603
- Feature-based CQRS organization in Application folders (e.g., Medical.Application/Hospitals/Commands)

## Testing
- Unit tests: `dotnet test --filter "FullyQualifiedName~MyApp.UnitTests"`
- Integration tests: `dotnet test --filter "FullyQualifiedName~MyApp.IntegrationTests"` (requires Docker)
- Integration tests share database - avoid parallel execution without isolation

## Cross-Module Communication (No Direct Infrastructure References)
- Appointments → Medical: `IPatientRepository`
- Appointments → Scheduling: `IAppointmentSlotRepository`
- Appointments → Insurance: `IPatientInsuranceRepository`
- Billing → Appointments: `IAppointmentRepository`
- Scheduling → Medical: `IDoctorHospitalRepository`
- Medical (Patients) → Identity: `IUserRepository`

## Common Gotchas
1. Table names pluralized (`clinics` not `clinic`)
2. Migration order is fixed in Program.cs - add new modules there
3. JWT validation fails if SecretKey < 32 chars
4. Rate limiting runs before authentication (applies to unauthenticated requests)
5. `UseOpenTelemetryPrometheusScrapingEndpoint()` must follow `AddApplicationObservability()`
6. Swagger disabled in Production by default
7. `AddSharedInfrastructure()` must precede module registrations

## Database
- EF Core with Npgsql (PostgreSQL)
- Connection string: `DefaultConnection` (localhost:5432)
- Each module's Infrastructure has separate migrations assembly
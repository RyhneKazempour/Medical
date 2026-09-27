namespace MyApp.Insurance.Application.Abstractions;

using MyApp.Insurance.Domain.Entities;
using MyApp.Shared.Application.Abstractions;

public interface IInsuranceRepository : IRepository<Insurance>
{
    Task<Insurance?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

public interface IPatientInsuranceRepository : IRepository<PatientInsurance>
{
    Task<IReadOnlyList<PatientInsurance>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<PatientInsurance?> GetPrimaryByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<PatientInsurance?> GetByPatientAndInsuranceAsync(Guid patientId, Guid insuranceId, CancellationToken cancellationToken = default);
}

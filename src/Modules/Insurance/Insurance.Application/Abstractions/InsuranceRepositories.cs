using Insurance.Domain.Entities;
using Shared.Application.Abstractions;

namespace Insurance.Application.Abstractions;
public interface IInsuranceRepository : IRepository<Insurance.Domain.Entities.InsuranceInfo>
{
    Task<Insurance.Domain.Entities.InsuranceInfo?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

public interface IPatientInsuranceRepository : IRepository<PatientInsurance>
{
    Task<IReadOnlyList<PatientInsurance>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<PatientInsurance?> GetPrimaryByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<PatientInsurance?> GetByPatientAndInsuranceAsync(Guid patientId, Guid insuranceId, CancellationToken cancellationToken = default);
}

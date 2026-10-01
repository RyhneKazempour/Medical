namespace MyApp.Medical.Application.Abstractions;

using MyApp.Medical.Domain.Entities;
using MyApp.Shared.Application.Abstractions;

public interface IHospitalRepository : IRepository<Hospital>
{
    Task<Hospital?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}

public interface IClinicRepository : IRepository<Clinic>
{
    Task<IReadOnlyList<Clinic>> GetByHospitalIdAsync(Guid hospitalId, CancellationToken cancellationToken = default);
}

public interface ISpecializationRepository : IRepository<Specialization>
{
    Task<Specialization?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<Doctor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Doctor>> GetBySpecializationIdAsync(Guid specializationId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IDoctorSpecializationRepository : IRepository<DoctorSpecialization>
{
    Task<IReadOnlyList<DoctorSpecialization>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorSpecialization>> GetBySpecializationIdAsync(Guid specializationId, CancellationToken cancellationToken = default);
    Task<DoctorSpecialization?> GetByDoctorAndSpecializationAsync(Guid doctorId, Guid specializationId, CancellationToken cancellationToken = default);
}

public interface IDoctorHospitalRepository : IRepository<DoctorHospital>
{
    Task<IReadOnlyList<DoctorHospital>> GetByDoctorSpecializationIdAsync(Guid doctorSpecializationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorHospital>> GetByClinicIdAsync(Guid clinicId, CancellationToken cancellationToken = default);
    Task<DoctorHospital?> GetByDoctorSpecializationAndClinicAsync(Guid doctorSpecializationId, Guid clinicId, CancellationToken cancellationToken = default);
}

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

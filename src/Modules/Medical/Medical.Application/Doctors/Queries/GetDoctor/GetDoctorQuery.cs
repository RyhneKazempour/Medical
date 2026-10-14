namespace Medical.Application.Doctors.Queries.GetDoctor;

using MediatR;
using Shared.Domain.Common;

public sealed record GetDoctorQuery(Guid DoctorId) : IRequest<Result<DoctorResponse>>;


public sealed record ListDoctorsQuery : IRequest<Result<IReadOnlyList<DoctorResponse>>>;

public sealed record ListDoctorsBySpecializationQuery(Guid SpecializationId) : IRequest<Result<IReadOnlyList<DoctorResponse>>>;

public sealed record ListDoctorsByHospitalQuery(Guid HospitalId) : IRequest<Result<IReadOnlyList<DoctorResponse>>>;
namespace Medical.Application.Hospitals.Queries.GetHospital;

using MediatR;
using Shared.Domain.Common;

public sealed record GetHospitalQuery(Guid HospitalId) : IRequest<Result<HospitalResponse>>;

public sealed record ListHospitalsQuery : IRequest<Result<IReadOnlyList<HospitalResponse>>>;

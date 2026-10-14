namespace MyApp.M.Application.Patients.Queries.GetPatient;

using MediatR;
using Medical.Application.Patients.Queries.GetPatient;
using Shared.Domain.Common;

public sealed record ListPatientsQuery : IRequest<Result<IReadOnlyList<PatientResponse>>>;

namespace Medical.Application.Patients.Commands.SoftDeletePatient;

using MediatR;
using Shared.Domain.Common;

public sealed record SoftDeletePatientCommand(Guid Id) : IRequest<Result>;
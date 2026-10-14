namespace Medical.Application.Patients.Commands.RestorePatient;

using MediatR;
using Shared.Domain.Common;

public sealed record RestorePatientCommand(Guid Id) : IRequest<Result>;
namespace Medical.Application.Doctors.Commands.RestoreDoctor;

using MediatR;
using Shared.Domain.Common;

public sealed record RestoreDoctorCommand(Guid Id) : IRequest<Result>;
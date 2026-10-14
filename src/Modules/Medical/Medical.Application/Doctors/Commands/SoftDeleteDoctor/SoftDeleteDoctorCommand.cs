namespace Medical.Application.Doctors.Commands.SoftDeleteDoctor;

using MediatR;
using Shared.Domain.Common;

public sealed record SoftDeleteDoctorCommand(Guid Id) : IRequest<Result>;
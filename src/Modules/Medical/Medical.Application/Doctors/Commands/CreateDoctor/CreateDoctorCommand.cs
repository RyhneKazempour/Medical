namespace Medical.Application.Doctors.Commands.CreateDoctor;

using MediatR;
using Shared.Domain.Common;

public sealed record CreateDoctorCommand(Guid UserId, string LicenseNumber, string? Bio)
    : IRequest<Result<Guid>>;

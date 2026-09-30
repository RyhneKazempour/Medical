namespace MyApp.Medical.Application.Commands;

using MediatR;
using MyApp.Shared.Domain;

public sealed record CreateDoctorCommand(Guid UserId, string LicenseNumber, string? Bio)
    : IRequest<Result<Guid>>;
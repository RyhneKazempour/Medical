namespace Medical.Application.Doctors.Commands.UpdateDoctor;

using MediatR;
using Medical.Application.Doctors.Queries.GetDoctor;
using Shared.Domain.Common;

public sealed record UpdateDoctorCommand(
    Guid Id,
    string? LicenseNumber,
    string? Bio,
    bool? IsActive)
    : IRequest<Result<DoctorResponse>>;

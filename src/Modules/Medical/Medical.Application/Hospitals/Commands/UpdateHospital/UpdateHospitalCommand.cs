namespace Medical.Application.Hospitals.Commands.UpdateHospital;

using MediatR;
using Medical.Application.Hospitals.Queries.GetHospital;
using Shared.Domain.Common;

public sealed record UpdateHospitalCommand(
    Guid Id,
    string? Name,
    string? Address,
    string? Phone,
    string? Email,
    bool? IsActive)
    : IRequest<Result<HospitalResponse>>;

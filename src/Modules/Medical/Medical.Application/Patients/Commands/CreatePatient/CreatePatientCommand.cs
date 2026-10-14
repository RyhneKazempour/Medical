namespace Medical.Application.Patients.Commands.CreatePatient;

using MediatR;
using Shared.Domain.Common;

public sealed record CreatePatientCommand(
    Guid UserId,
    DateOnly? DateOfBirth,
    string? Gender,
    string? BloodType,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Address)
    : IRequest<Result<Guid>>;
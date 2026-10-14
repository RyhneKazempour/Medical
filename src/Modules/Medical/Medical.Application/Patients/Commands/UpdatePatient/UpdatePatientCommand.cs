namespace Medical.Application.Patients.Commands.UpdatePatient;

using MediatR;
using Medical.Application.Patients.Queries.GetPatient;
using Shared.Domain.Common;

public sealed record UpdatePatientCommand(
    Guid Id,
    DateOnly? DateOfBirth,
    string? Gender,
    string? BloodType,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Address)
    : IRequest<Result<PatientResponse>>;

namespace MyApp.Medical.Application.Authentication;

using MyApp.Medical.Domain.Entities;

public sealed record DoctorResponse(
    Guid Id,
    Guid UserId,
    string LicenseNumber,
    string? Bio);

public sealed record PatientResponse(
    Guid Id,
    Guid UserId,
    DateOnly? DateOfBirth,
    string? Gender,
    string? BloodType,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Address);

public sealed record HospitalResponse(
    Guid Id,
    string Name,
    string? Address,
    string? Phone,
    string? Email);

public sealed record ClinicResponse(
    Guid Id,
    Guid HospitalId,
    string Name,
    string? Description);

public sealed record SpecializationResponse(
    Guid Id,
    string Name,
    string? Description);

public sealed record DoctorSpecializationResponse(
    Guid Id,
    Guid DoctorId,
    Guid SpecializationId);

public sealed record DoctorHospitalResponse(
    Guid Id,
    Guid DoctorSpecializationId,
    Guid ClinicId,
    string? RoomNumber,
    DateOnly StartContractDate,
    DateOnly? EndContractDate);
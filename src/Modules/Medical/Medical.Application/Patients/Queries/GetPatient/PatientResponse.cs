namespace Medical.Application.Patients.Queries.GetPatient;


public sealed record PatientResponse(
    Guid Id,
    Guid UserId,
    DateOnly? DateOfBirth,
    string? Gender,
    string? BloodType,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Address);


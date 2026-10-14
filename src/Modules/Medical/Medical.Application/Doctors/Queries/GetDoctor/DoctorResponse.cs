namespace Medical.Application.Doctors.Queries.GetDoctor;

public sealed record DoctorResponse(
    Guid Id,
    Guid UserId,
    string LicenseNumber,
    string? Bio);

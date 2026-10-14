namespace Medical.Application.Hospitals.Queries.GetHospital;

public sealed record HospitalResponse(
    Guid Id,
    string Name,
    string? Address,
    string? Phone,
    string? Email);
